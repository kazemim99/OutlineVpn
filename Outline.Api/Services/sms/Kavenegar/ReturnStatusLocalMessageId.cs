using Outline.Api.Services.sms.Kavenegar.Models;
using System.Collections.Generic;

namespace Outline.Api.Services.sms.Kavenegar
{
    internal class ReturnStatusLocalMessageId
    {
        public Result result { get; set; }

        public List<StatusLocalMessageIdResult> entries { get; set; }
    }
}