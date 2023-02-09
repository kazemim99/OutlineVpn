using static V2Ray.Api.Entity.SSHKey;
using static V2Ray.Api.Services.Server.ServerService;

namespace V2Ray.Api.Entity
{
    public class V2Server : FullAuditEntity<int>, ISoftDelete
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string IP { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get;  set; }
        public List<SSHKey> SSHKeys { get; set; }
    }
    public class SSHKey:AuditEntity<int>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime ExpireDate { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public int Port { get; set; }
        public bool Enable { get;  set; }
        public int? ServerId { get; set; }
        public V2Server V2Server { get; set; }
    public class Country : FullAuditEntity<int>, ISoftDelete
    {
        public string Title { get; set; }
        public string Flag { get; set; }
        public List<City> Cities { get; set; }
        public bool IsDeleted { get; set; }
      }
    }

    public class City : FullAuditEntity<int>, ISoftDelete
    {
        public string Title { get; set; }

        public bool IsDeleted { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public List<V2Server> V2Servers { get; set; }
    }
    public class V2Key : FullAuditEntity<int>, ISoftDelete
    {
        public string Remark { get; set; }
        public int Traffic { get; set; }
        public int ClientPort { get; set; }
        public long ExpireDate { get; set; }
        public bool State { get; set; } = true;
        public Protocol Protocol { get; set; }
        public string Key { get; set; }
        public bool IsDeleted { get; set; }
        public int V2ServerId { get; set; }
        public V2Server V2Server { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public int Port { get; internal set; }
        public int KeyId { get;  set; }
    }
}