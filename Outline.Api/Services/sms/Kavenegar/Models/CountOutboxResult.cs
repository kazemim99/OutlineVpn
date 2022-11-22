namespace Outline.Api.Services.sms.Kavenegar.Models
{
    public class CountOutboxResult : CountInboxResult
    {
        public long SumPart { get; set; }

        public long Cost { get; set; }
    }
}