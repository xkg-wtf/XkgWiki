using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using XkgWiki.Data.Stores;
using XkgWiki.Shared;

namespace XkgWiki.Data
{
    public class DataDiRegistrationModule : DiRegistrationModuleBase
    {
        public DataDiRegistrationModule(
            Func<IRegistrationBuilder<object, object, object>, IRegistrationBuilder<object, object, object>> lifetimeScopeConfigurator) : base(
            lifetimeScopeConfigurator)
        {
        }

        protected override IEnumerable<IRegistrationBuilder<object, object, object>> RegisterTypesWithDefaultLifetimeScope(ContainerBuilder builder)
        {
            yield return builder.RegisterAssemblyTypes(typeof(EntityRepositoryBase<,>).Assembly).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces();

            yield return builder.Register(c =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<XkgDbContext>();
                optionsBuilder.UseNpgsql(c.Resolve<IConfiguration>().GetConnectionString(XkgDbContext.ConnectionStringName));

                return optionsBuilder.Options;
            }).As<DbContextOptions>();

            yield return builder.RegisterType<XkgDbContext>().AsSelf();
            yield return builder.RegisterType<UnitOfWork>().AsImplementedInterfaces();

            yield return builder.RegisterType<UserStore>().AsImplementedInterfaces();
            yield return builder.RegisterType<RoleStore>().AsImplementedInterfaces();
        }
    }
}