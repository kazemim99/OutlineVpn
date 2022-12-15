using System.Collections.Generic;
using V2Ray.Api.Services.sms.Kavenegar.Models;

namespace V2Ray.Api.Services.sms.Kavenegar
{
    internal class ReturnCountPostalCode
    {
        public Result result { get; set; }

        public List<CountPostalCodeResult> entries { get; set; }
    }
}