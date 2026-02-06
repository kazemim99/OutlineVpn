namespace V2Ray.Api.Services.SSHKeyServices
{
    public class SSHKeyServiceConfiguration
    {
        public SshConfiguration Ssh { get; set; } = new();
        public PanelConfiguration Panel { get; set; } = new();
        public List<string> NodeIps { get; set; } = new();
        public UserTrafficConfiguration UserTraffic { get; set; } = new();
    }

    public class SshConfiguration
    {
        public int Port { get; set; } = 1027;
        public string Username { get; set; } = "root";
        public string Password { get; set; } = string.Empty;
    }

    public class PanelConfiguration
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string BaseDomain { get; set; } = "p.iransshvpn.com";
    }

    public class UserTrafficConfiguration
    {
        public Dictionary<int, int> OneMonthTraffic { get; set; } = new();
        public Dictionary<int, int> TwoMonthTraffic { get; set; } = new();
        public Dictionary<int, int> ThreeMonthTraffic { get; set; } = new();

        public int DefaultOneMonthTraffic { get; set; } = 55;
        public int DefaultTwoMonthTraffic { get; set; } = 110;
        public int DefaultThreeMonthTraffic { get; set; } = 165;

        public int GetTrafficForDuration(int userId, int months)
        {
            return months switch
            {
                1 => OneMonthTraffic.GetValueOrDefault(userId, DefaultOneMonthTraffic),
                2 => TwoMonthTraffic.GetValueOrDefault(userId, DefaultTwoMonthTraffic),
                3 => ThreeMonthTraffic.GetValueOrDefault(userId, DefaultThreeMonthTraffic),
                _ => DefaultOneMonthTraffic
            };
        }
    }

    public class ChargeValidationException : Exception
    {
        public ChargeValidationException(string message) : base(message) { }
    }
}
