using V2Ray.Api.Services.sms.Kavenegar.Models;

namespace V2Ray.Api.Services.sms.Kavenegar
{
    internal class ReturnAccountInfo
    {
        public Result result { get; set; }

        public AccountInfoResult entries { get; set; }
    }
}