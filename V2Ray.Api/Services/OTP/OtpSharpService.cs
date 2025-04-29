using Microsoft.Extensions.Options;
using OtpNet;
using System.Text;
using V2Ray.Api.Services.OTP.DTO;
using V2Ray.Api.Services.Settings;

namespace V2Ray.Api.Services.OTP
{
    public class OtpSharpService : OtpServiceBase, IOtpService
    {
        public OtpSharpService(IOptions<OtpSettings> settings) : base(settings.Value)
        {
        }

        public Totp GetCode(string key, int? stepWindowSeconds = null)
        {
            key = key.TrimStart(new[] { '0' });

            var totp = GetTotp(key, _settings.StepWindow);
            return totp;
        }

        public OtpVerifyOut VerifyCode(string key, string code)
        {

                key = key.TrimStart(new[] { '0' });
                var totp = GetTotp(key);

                var result = totp.VerifyTotp(code, out long timeStepMatched,
                VerificationWindow.RfcSpecifiedNetworkDelay);
                var resp = new OtpVerifyOut(result,
                 (int)(timeStepMatched / 1000));

                if (!resp.Matched || resp.MatchTime <= 0)
                    throw new Exception("کد ارسال شده اشتباه است");

                return resp;
        }

        private Totp GetTotp(string key, int? stepWindowSeconds = null)
        {
            stepWindowSeconds ??= _settings.StepWindow;
            var secretKey = Encoding.UTF8.GetBytes(_settings.SecretKey + key);
            return new Totp(secretKey, stepWindowSeconds.Value, OtpHashMode.Sha256, _settings.Size);
        }
    }
}