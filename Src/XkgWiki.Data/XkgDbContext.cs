using Att.Domain;
using Microsoft.EntityFrameworkCore;
using XkgWiki.Domain;

namespace XkgWiki.Data
{
    public class XkgDbContext : DbContext
    {
        public const string ConnectionStringName = "AttConnectionString";

        public DbSet<Claim> Claims { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<Page> Pages { get; set; }

        public XkgDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Page>()
                .HasAlternateKey(p => p.Url);
        }
    }
}