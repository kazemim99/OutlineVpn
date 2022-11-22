using Outline.Api.Services.sms.Kavenegar.Models;
using System.Collections.Generic;

namespace Outline.Api.Services.sms.Kavenegar
{
    internal class ReturnCountOutbox
    {
        public Result result { get; set; }

        public List<CountOutboxResult> entries { get; set; }
    }
}