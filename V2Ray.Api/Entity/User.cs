using System.Collections.Generic;

namespace V2Ray.Api.Entity
{

    public class User : FullAuditEntity<int>, ISoftDelete
    {
        public User()
        {

        }
        public bool UsedFreeAccount { get; set; }
        public string Email { get; set; }
        public string? Avatar { get; set; }
        public string  IP { get; set; }

        //public string Password { get; set; }

        public bool IsAdmin { get; set; }
        public bool Enable { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<UserRole> Roles { get; set; }
        public string Password { get;  set; }
        public bool NeedConfirm { get;  set; }
        public string? FirstName { get;  set; }
        public string? LastName { get;  set; }
        public List<SSHKeyInfo> SSHKeyInfos { get; set; }
        public List<V2Key> V2Keys { get; set; }
        public bool Paid { get;  set; }
        public List<ProblemReport> ProblemReports { get;  set; }
    }
}