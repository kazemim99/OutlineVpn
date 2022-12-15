namespace V2Ray.Api.Services.Settings
{
    public class OtpSettings
    {
        public int StepWindow { get; set; }

        public int Size { get; set; }

        public string SecretKey { get; set; }

        public bool Sandbox { get; set; }

        public string SandboxCode { get; set; }
    }
}