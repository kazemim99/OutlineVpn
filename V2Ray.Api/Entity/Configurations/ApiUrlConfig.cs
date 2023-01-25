using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace V2Ray.Api.Entity.Configurations
{
    public class ApiUrlConfig : IEntityTypeConfiguration<V2Server>
    {
        public void Configure(EntityTypeBuilder<V2Server> builder)
        {
            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }

    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasQueryFilter(a => !a.IsDeleted);
            builder.HasOne(a => a.User).WithMany(a => a.Orders).HasForeignKey(c => c.UserId);
        }
    }
    public class ProblemReportConfig : IEntityTypeConfiguration<ProblemReport>
    {
        public void Configure(EntityTypeBuilder<ProblemReport> builder)
        {
            builder.ToTable("ProblemReports");
            builder.HasQueryFilter(a => !a.IsDeleted);
            builder.HasOne(a => a.User).WithMany(a => a.ProblemReports).HasForeignKey(c => c.UserId);
        }
    }
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
    public class PlanConfig : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
}
