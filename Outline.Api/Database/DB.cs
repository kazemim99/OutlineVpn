using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Outline.Api.Entity;
using Outline.Api.Entity.Configurations;

namespace Outline.Api.Database
{

    public class DesignDBFactory : IDesignTimeDbContextFactory<DB>
    {
        public DB CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DB>();

            //var connectionString = "Data Source=185.55.224.120;Initial Catalog= powerbox_sa;User ID=powerbox_sa;Password=Pa$$w0rd";
            var connectionString = "Server=.; Database=Outline; Trusted_Connection=true;";

            builder.UseSqlServer(connectionString);
            return new DB(builder.Options);
        }
    }
    public class DB : DbContext
    {

        public DB(DbContextOptions<DB> options) : base(options)
        {
        }

        public DbSet<Plan> Plans { get; set; }
        public DbSet<FailedSms> FailedSms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ApiUrl> ApiUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfig).Assembly);
        }
    }
}