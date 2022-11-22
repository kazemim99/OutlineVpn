using Outline.Api.Services.sms.Kavenegar.Models;
using System.Collections.Generic;

namespace Outline.Api.Services.sms.Kavenegar
{
    internal class ReturnCountPostalCode
    {
        public Result result { get; set; }

        public List<CountPostalCodeResult> entries { get; set; }
    }
}