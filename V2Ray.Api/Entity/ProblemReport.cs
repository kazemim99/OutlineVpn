using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Entity
{
    public class ProblemReport : FullAuditEntity<int>, ISoftDelete
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public OperatorEnum Operator { get; set; }
        public OSEnum OS { get; set; }
        public string? Despriction { get; set; }
        public bool ReturnMoney { get; set; }
        public bool IsDeleted
        {
            get; set;

        }
        public string? Answer { get; set; }

        public ProblemReportEnum State { get; set; }
    }
}