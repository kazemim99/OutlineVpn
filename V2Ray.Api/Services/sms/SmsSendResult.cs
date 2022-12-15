using System;
using V2Ray.Api.Services.sms.Kavenegar.Utils;

namespace V2Ray.Api.Services.sms
{
    public class SmsSendResult
    {
        public long MessageId { get; set; }

        public int Cost { get; set; }

        public long Date { get; set; }

        public string Message { get; set; }

        public string Receptor { get; set; }

        public string Sender { get; set; }

        public int Status { get; set; }

        public string StatusText { get; set; }

        public DateTime GregorianDate
        {
            get { return DateHelper.UnixTimestampToDateTime(Date); }
            set { Date = DateHelper.DateTimeToUnixTimestamp(value); }
        }
    }
}