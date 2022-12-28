using static V2Ray.Api.Services.Server.ServerService;

namespace V2Ray.Api.Entity
{
    public class V2Server : FullAuditEntity<int>, ISoftDelete
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
        public City City { get; set; }
        public string IP { get; set; }
        public bool IsDeleted { get; set; }
        public List<V2Key> Keys { get; set; }
        public int CityId { get; set; }
        public bool IsActive { get; set; }
        public bool Swapped { get;  set; }
        public bool IsMain { get;  set; }
    }

    public class Country : FullAuditEntity<int>, ISoftDelete
    {
        public string Title { get; set; }
        public string Flag { get; set; }
        public List<City> Cities { get; set; }
        public bool IsDeleted { get; set; }
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
        public long Capacity { get; set; }
        public string? ClientKeyId { get; set; }
        public long ExpireDate { get; set; }
        public bool State { get; set; }
        public Protocol Protocol { get; set; }
        public string Key { get; set; }
        public bool IsDeleted { get; set; }
        public int V2ServerId { get; set; }
        public V2Server V2Server { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public int Port { get; internal set; }
    }
}