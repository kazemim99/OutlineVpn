using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.ProblemReportServices.Dto
{
    public class GetProblemReportListOutput
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Operator { get; set; }
        public string OS { get; set; }
        public string ReturnMoney { get; set; }
        public string CreatedAt { get; set; }
        public string Answer { get; set; }
        public string State { get; set; }
    }
}