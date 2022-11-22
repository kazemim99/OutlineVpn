using Outline.Api.Services.Settings;
using Outline.Api.Services.sms.Kavenegar;
using Outline.Api.Services.sms.Kavenegar.Exceptions;
using Outline.Api.Services.sms.Kavenegar.Models;

namespace Outline.Api.Services.sms
{
    public class KavanegarSender /*: ISmsSender*/
    {
        private readonly SmsSettings _smsSetting;

        public KavanegarSender(SmsSettings smsSetting)
        {
            _smsSetting = smsSetting;
        }

        public async Task SendAsync(SMSRequest request)
        {
            var api = new KavenegarApi(_smsSetting.Kavenegar.ApiKey);
            try
            {
                var result = await api.Send(_smsSetting.Kavenegar.SenderLine, request.Receptor, request.Message);
            }
            catch (ApiException ex)
            {
                // در صورتی که خروجی وب سرویس 200 نباشد این خطارخ می دهد.
                throw new ApiException(ex.Message, 2);
            }
            catch (HttpException ex)
            {
                // در زمانی که مشکلی در برقرای ارتباط با وب سرویس وجود داشته باشد این خطا رخ می دهد
                throw new HttpException(ex.Message, 2);
            }
        }
    }
}