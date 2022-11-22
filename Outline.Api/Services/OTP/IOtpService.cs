using Outline.Api.Services.OTP.DTO;

namespace Outline.Api.Services.OTP
{
    public interface IOtpService
    {
        bool Sandbox { get; set; }

        string GetCode(string key, int? stepWindowSeconds = null);

        OtpVerifyOut VerifyCode(string key, string code);
    }
}