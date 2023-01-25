using System.Collections.Generic;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.ProblemReportServices.Dto
{
    public class GetProblemReportOutput : EntityDto<int>
    {
        public string UserName { get; set; }
        public OperatorEnum Operator { get; set; }
        public OSEnum OS { get; set; }
        public string CreateDate { get; set; }

        public string Despriction { get; set; }
    }
}