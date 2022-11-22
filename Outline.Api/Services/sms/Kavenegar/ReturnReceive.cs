using Outline.Api.Services.sms.Kavenegar.Models;
using System.Collections.Generic;

namespace Outline.Api.Services.sms.Kavenegar
{
    internal class ReturnReceive
    {
        public Result result { get; set; }

        public List<ReceiveResult> entries { get; set; }
    }
}