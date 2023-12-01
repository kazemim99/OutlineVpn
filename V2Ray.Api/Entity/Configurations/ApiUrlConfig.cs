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
            builder.Property(a => a.CreatorUserId).IsRequired(false);
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
    public class SSHKeyConfig : IEntityTypeConfiguration<SSHKey>
    {
        public void Configure(EntityTypeBuilder<SSHKey> builder)
        {
            builder.HasIndex(c => c.UserName)
                .IsUnique();
            builder.Property(c => c.UserName).IsRequired();
            builder.Property(c => c.Password).IsRequired();

            builder.HasOne(a => a.V2Server).WithMany(c => c.SSHKeys)
                .HasForeignKey(b => b.ServerId).IsRequired(true);

            builder.HasOne(a => a.User).WithMany(c => c.SSHKeyInfos)
            .HasForeignKey(b => b.UserId).IsRequired(true);
        }
    }

    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasMany(t => t.Messages)
                .WithOne(m => m.Ticket)
                .HasForeignKey(m => m.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasMany(m => m.Attachments)
                .WithOne(a => a.Message)
                .HasForeignKey(a => a.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

            builder.HasQueryFilter(a => !a.IsDeleted);
            builder.Property(a => a.Password).IsRequired(false);
            builder.Property(a => a.Email).IsRequired(false);
        }
    }

    public class V2ServerConfig : IEntityTypeConfiguration<V2Server>
    {
        public void Configure(EntityTypeBuilder<V2Server> builder)
        {
            builder.HasQueryFilter(a => !a.IsDeleted);
            builder.HasOne(a => a.User).WithMany(a=>a.Servers).HasForeignKey(c=>c.UserId).IsRequired(true);
            builder.HasMany(a => a.SSHKeys).WithOne(a=>a.V2Server).HasForeignKey(c=>c.ServerId).IsRequired(true);
            builder.Property(a => a.Capacity).HasDefaultValue(50);
        }
    }
}
