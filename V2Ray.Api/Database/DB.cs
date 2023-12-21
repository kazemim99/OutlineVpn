using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using V2Ray.Api.Entity;
using V2Ray.Api.Entity.Configurations;
using static V2Ray.Api.Entity.SSHKey;

namespace V2Ray.Api.Database
{

    public class DesignDBFactory : IDesignTimeDbContextFactory<DB>
    {
        public DB CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Get connection string from appsettings.json
            string connectionString = configuration.GetConnectionString("Default");

            // Create options builder using Npgsql
            var optionsBuilder = new DbContextOptionsBuilder<DB>()
                .UseNpgsql(connectionString);

            return new DB(optionsBuilder.Options);
        }
    }
    public class DB : DbContext
    {

        public DB(DbContextOptions<DB> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<ProblemReport> ProblemReports { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<V2Server> V2Servers { get; set; }
        public DbSet<V2Key> V2Keys { get; set; }
        public DbSet<SSHKey> SSHKeyInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfig).Assembly);
        }
    }
}