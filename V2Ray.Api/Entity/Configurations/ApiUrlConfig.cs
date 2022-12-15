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
