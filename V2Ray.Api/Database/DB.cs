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
            var builder = new DbContextOptionsBuilder<DB>();

            var connectionString = "Data Source=185.159.153.204 ;Initial Catalog= iranvra1_sa ;User ID=iranvra1_sa;Password=!Q@W#E$R5t6y7u8i";
            //var connectionString = "Server=.; Database=V2Ray;Integrated Security=True";

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
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProblemReport> ProblemReports { get; set; }
        public DbSet<FailedSms> FailedSms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<V2Server> V2Servers { get; set; }
        public DbSet<V2Key> V2Keys { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<SSHKey> SSHKeyInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfig).Assembly);
        }
    }
}