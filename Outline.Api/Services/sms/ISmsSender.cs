using Outline.Api.Services.sms.Rahyab;

namespace Outline.Api.Services.sms
{
    public interface IRahyabSmsSender
    {
        public Task SendAsync(RahyabSendSmsReques request);
        void SendEmail(string code, string to);

    }
}