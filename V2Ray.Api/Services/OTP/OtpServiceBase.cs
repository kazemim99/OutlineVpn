using V2Ray.Api.Services.Settings;

namespace V2Ray.Api.Services.OTP
{
    public abstract class OtpServiceBase
    {
        protected OtpSettings _settings;

        public bool Sandbox { get; set; }

        public OtpServiceBase(OtpSettings settings)
        {
            _settings = settings;
            Sandbox = _settings.Sandbox;
        }
    }
}