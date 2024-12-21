namespace V2Ray.Api.Services.SSHKeyServices
{
    public class ConfigDateOutput
    {
        public int Port { get; set; }
        public int Domain { get; set; }
        public int SubId { get; set; }
        public int VmessSubId { get;  set; }
        public int VmessPort { get;  set; }
        public int IranSubId => 2;
    }
}