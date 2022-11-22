namespace Outline.Api.Services.Settings
{
    public class SmsSettings
    {
        public int SenderClass { get; set; }

        public Kavenegar Kavenegar { get; set; }

        public string SecretKey { get; set; }

        public bool Sandbox { get; set; }

        public string SandboxCode { get; set; }
    }
}