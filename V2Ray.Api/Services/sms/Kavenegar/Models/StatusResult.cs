using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.sms.Kavenegar.Models
{
    public class StatusResult
    {
        public long Messageid { get; set; }

        public MessageStatus Status { get; set; }

        public string Statustext { get; set; }
    }
}