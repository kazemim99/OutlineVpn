using Outline.Api.Services.sms.Kavenegar.Models;
using System.Collections.Generic;

namespace Outline.Api.Services.sms.Kavenegar
{
    internal class ReturnSend
    {
        public Result @Return { get; set; }

        public List<SendResult> entries { get; set; }
    }
}