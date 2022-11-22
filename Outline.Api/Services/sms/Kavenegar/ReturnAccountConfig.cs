using Outline.Api.Services.sms.Kavenegar.Models;

namespace Outline.Api.Services.sms.Kavenegar
{
    internal class ReturnAccountConfig
    {
        public Result result { get; set; }

        public AccountConfigResult entries { get; set; }
    }
}