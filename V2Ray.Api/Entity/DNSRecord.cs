namespace V2Ray.Api.Entity
{
    public class DNSRecord : Entity<int>
    {
        public string Ipv4 { get; set; }
        public string Ipv6 { get; set; }
    }
}