using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.ProblemReports.Dto
{
    public class GetProblemReportListOutput
    {
        public string UserName { get; set; }
        public OperatorEnum Operator { get; set; }
        public OSEnum OS { get; set; }
        public string CreatedAt { get; set; }
    }
}