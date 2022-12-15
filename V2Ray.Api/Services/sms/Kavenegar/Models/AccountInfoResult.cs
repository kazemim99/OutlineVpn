using System;

namespace V2Ray.Api.Services.sms.Kavenegar.Models
{
    public class AccountInfoResult
    {
        public long RemainCredit { get; set; }

        public long Expiredate { get; set; }

        public DateTime GregorianExpiredate
        {
            get { return Utils.DateHelper.UnixTimestampToDateTime(Expiredate); }
        }

        public string Type { get; set; }
    }
}