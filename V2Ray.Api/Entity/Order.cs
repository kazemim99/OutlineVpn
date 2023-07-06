using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Entity
{
    public class Order : FullAuditEntity<int>, ISoftDelete
    {
        public SSHKey SSHKey { get; set; }
        public int SSHKeyId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public OrderStateEnum Status { get; set; }

        public bool IsDeleted
        {
            get; set;

        }
        public int MonthCount { get; internal set; }
    }
}