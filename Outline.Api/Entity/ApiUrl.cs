namespace Outline.Api.Entity
{
    public class ApiUrl : FullAuditEntity<int>, ISoftDelete
    {
        public string Title { get; set; }
        public string? Url { get; set; }
        public string Country { get; set; }
        public string IP { get; set; }
        public bool State { get; set; }
        public bool IsDeleted { get; set; }
        public List<User> Users { get;  set; }
    }

    public class UserServer
    {
        public User User { get; set; }
        public ApiUrl Server { get; set; }
        public int ServerId { get; set; }
        public int UserId { get; set; }
    }
}