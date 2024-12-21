using System.Reflection.Metadata.Ecma335;

namespace V2Ray.Api.Entity
{
    public class DNSRecord : Entity<int>
    {
        public string Ipv4 { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public bool Enable { get;  set; }
    }
}