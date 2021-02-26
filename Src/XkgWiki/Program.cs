using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using XkgWiki.Data;
using XkgWiki.Shared.Extensions;

namespace XkgWiki
{
    public class Program
    {
        private static IConfiguration _configuration;

        public static async Task Main(string[] args)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var envConfigName = $"appsettings.{envName}.json";

            SetupConfigurationIfNotExists(b =>
            {
                b.AddCommandLine(args);
                b.AddJsonFile(envConfigName, true);
            });

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Debug(outputTemplate: DateTime.Now.ToString(CultureInfo.InvariantCulture))
                .CreateLogger();

            if (envName.IsNullOrEmpty())
                Log.Logger.Error("Environment name isn't provided.");
            else if (!File.Exists(Path.Combine(Environment.CurrentDirectory, envConfigName)))
                Log.Logger.Warning("Be careful! Configuration file for environment '{EnvConfigName}' doesn't exist.", envConfigName);

            await MigrateDbAsync();
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            SetupConfigurationIfNotExists();

            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(_configuration))
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog();
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(w => w.UseStartup<Startup>());
        }

        private static void SetupConfigurationIfNotExists(Action<IConfigurationBuilder> action = null)
        {
            if (_configuration != null)
                return;

            var configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json");

            action?.Invoke(configurationBuilder);

            _configuration = configurationBuilder.Build();
        }

        private static async Task MigrateDbAsync()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(_configuration).As<IConfiguration>();
            builder.Register<DbContextOptions>(c =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<XkgDbContext>();
                optionsBuilder.UseNpgsql(c.Resolve<IConfiguration>().GetConnectionString(XkgDbContext.ConnectionStringName));

                return optionsBuilder.Options;
            });

            builder.RegisterType<XkgDbContext>().AsSelf();

            var container = builder.Build();

            var ctx = container.Resolve<XkgDbContext>();

            var pendingMigrations = (await ctx.Database.GetPendingMigrationsAsync()).ToArray();
            if (pendingMigrations.Any())
            {
                Log.Logger.Information("DbContext outdated. Migrating...");

                foreach (var migration in pendingMigrations)
                {
                    Log.Logger.Information("Migration: {Migration}", migration);
                }

                await ctx.Database.MigrateAsync();
                Log.Logger.Information("Migrations has been applied!");
            }

            await container.DisposeAsync();
        }
    }
}
