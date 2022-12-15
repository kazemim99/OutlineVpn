using System.Collections.Generic;
using V2Ray.Api.Services.sms.Kavenegar.Models;

namespace V2Ray.Api.Services.sms.Kavenegar
{
    internal class ReturnSend
    {
        public Result @Return { get; set; }

        public List<SendResult> entries { get; set; }
    }
}