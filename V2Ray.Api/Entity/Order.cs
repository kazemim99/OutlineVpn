using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Entity
{
    public class Order : FullAuditEntity<int>, ISoftDelete
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int Amount { get; set; }
        public string CardNumber { get; set; }
        public OrderStateEnum Status { get; set; }
        public string? TranactionNumber { get; set; }

        public bool IsDeleted
        {
            get; set;

        }
    }
}