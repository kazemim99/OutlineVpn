using System;
using V2Ray.Api.Services.sms.Kavenegar.Utils;

namespace V2Ray.Api.Services.sms.Kavenegar.Models
{
    public class ReceiveResult
    {
        public long Date { get; set; }

        public DateTime GregorianDate
        {
            get
            {
                return DateHelper.UnixTimestampToDateTime(Date);
            }
        }

        public long MessageId { get; set; }

        public string Sender { get; set; }

        public string Message { get; set; }

        public string Receptor { get; set; }
    }
}