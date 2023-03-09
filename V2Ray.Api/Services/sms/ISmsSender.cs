using V2Ray.Api.Services.sms.Rahyab;

namespace V2Ray.Api.Services.sms
{
    public interface IRahyabSmsSender
    {
        public Task SendAsync(RahyabSendSmsReques request);
    }
}