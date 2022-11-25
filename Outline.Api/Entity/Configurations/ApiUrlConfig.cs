using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Outline.Api.Entity.Configurations
{
    public class ApiUrlConfig : IEntityTypeConfiguration<ApiUrl>
    {
        public void Configure(EntityTypeBuilder<ApiUrl> builder)
        {
            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }

    public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(bc => new { bc.RoleId, bc.UserId });
        }
    }
    public class UserServerConfig : IEntityTypeConfiguration<UserServer>
    {
        public void Configure(EntityTypeBuilder<UserServer> builder)
        {
            builder.HasKey(bc => new { bc.ServerId, bc.UserId });
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
