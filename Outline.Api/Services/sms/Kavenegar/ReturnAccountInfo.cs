using Outline.Api.Services.sms.Kavenegar.Models;

namespace Outline.Api.Services.sms.Kavenegar
{
    internal class ReturnAccountInfo
    {
        public Result result { get; set; }

        public AccountInfoResult entries { get; set; }
    }
}