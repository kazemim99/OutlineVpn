using System.Collections.Generic;
using V2Ray.Api.Services.sms.Kavenegar.Models;

namespace V2Ray.Api.Services.sms.Kavenegar
{
    internal class ReturnReceive
    {
        public Result result { get; set; }

        public List<ReceiveResult> entries { get; set; }
    }
}