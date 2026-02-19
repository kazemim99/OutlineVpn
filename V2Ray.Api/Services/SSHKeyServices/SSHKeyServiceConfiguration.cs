namespace V2Ray.Api.Services.SSHKeyServices
{





    public class UserTrafficConfiguration
    {
        public Dictionary<int, int> OneMonthTraffic { get; set; } = new();
        public Dictionary<int, int> TwoMonthTraffic { get; set; } = new();
        public Dictionary<int, int> ThreeMonthTraffic { get; set; } = new();

        public int DefaultOneMonthTraffic { get; set; } = 45;
        public int DefaultTwoMonthTraffic { get; set; } = 90;
        public int DefaultThreeMonthTraffic { get; set; } = 135;

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
