using Outline.Api.Services.sms.Kavenegar.Models;
using System.Collections.Generic;

namespace Outline.Api.Services.sms.Kavenegar
{
    internal class ReturnStatus
    {
        public Result result { get; set; }

        public List<StatusResult> entries { get; set; }
    }
}