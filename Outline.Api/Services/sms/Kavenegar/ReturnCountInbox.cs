using Outline.Api.Services.sms.Kavenegar.Models;
using System.Collections.Generic;

namespace Outline.Api.Services.sms.Kavenegar
{
    internal class ReturnCountInbox
    {
        public Result result { get; set; }

        public List<CountInboxResult> entries { get; set; }
    }
}