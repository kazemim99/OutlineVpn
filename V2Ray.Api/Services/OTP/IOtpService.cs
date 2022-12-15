using V2Ray.Api.Services.OTP.DTO;

namespace V2Ray.Api.Services.OTP
{
    public interface IOtpService
    {
        bool Sandbox { get; set; }

        string GetCode(string key, int? stepWindowSeconds = null);

        OtpVerifyOut VerifyCode(string key, string code);
    }
}