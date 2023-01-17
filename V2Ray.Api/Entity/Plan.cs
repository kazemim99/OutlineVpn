using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Entity
{
    public class Plan : FullAuditEntity<int>, ISoftDelete
    {
        public string Title { get; set; }
        public string? Descrption { get; set; }
        public int Price { get; set; }
        public int Period { get; set; }
        public bool PlanState { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
        public int TrafficCapacity { get; set; }
    }
    public class ProblemReport : FullAuditEntity<int>, ISoftDelete
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public OperatorEnum Operator { get; set; }
        public OSEnum OS { get; set; }
        public string Despriction { get; set; }
        public bool IsDeleted
        {
            get; set;

        }
        public string Answer { get; set; }

        public ProblemReportEnum State { get;  set; }
    }
}