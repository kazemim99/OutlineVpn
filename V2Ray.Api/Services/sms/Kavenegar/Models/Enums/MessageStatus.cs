namespace V2Ray.Api.Services.sms.Kavenegar.Models.Enums
{
    public enum OSEnum
    {
        Android,
        IOS,
        Window,
        Linux,
        Max,
        Others
    }
    public enum OperatorEnum
    {
        Irancell,
        MCI,
        Rightel,
        Wifi,
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