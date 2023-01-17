using System.ComponentModel;

namespace V2Ray.Api.Services.sms.Kavenegar.Models.Enums
{
    public enum OSEnum : int
    {
        [Description("اندروید")]
        Android = 1,
        [Description("آیفون")]
        IOS,
        [Description("ویندوز")]
        Window,
        [Description("لینوکس")]
        Linux,
        [Description("مک")]
        Mac,
        [Description("سایز")]

        Others
    }
    public enum ProblemReportEnum
    {
        [Description("در حال بررسی")]
        Sended = 1,
        [Description("پاسخ داده شد")]
        Answerd = 2
    }
    public enum OperatorEnum : int
    {
        [Description("ایرانسل")]
        Irancell = 1,
        [Description("همراه اول")]
        MCI,
        [Description("رایتل")]
        Rightel,
        [Description("وای فای")]
        Wifi,
        [Description("سایز")]
        Others
    }
    public enum MessageStatus
    {
        Queued = 1,

        Schulded = 2,

        SentToCenter = 4,

        Delivered = 10,

        Undelivered = 11,

        Canceled = 13,

        Filtered = 14,

        Received = 50,

        Incorrect = 100
    }
}