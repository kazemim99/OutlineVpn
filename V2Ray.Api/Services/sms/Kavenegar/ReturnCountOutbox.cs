using System.Collections.Generic;
using V2Ray.Api.Services.sms.Kavenegar.Models;

namespace V2Ray.Api.Services.sms.Kavenegar
{
    internal class ReturnCountOutbox
    {
        public Result result { get; set; }

        public List<CountOutboxResult> entries { get; set; }
    }
}