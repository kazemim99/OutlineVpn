using System.Collections.Generic;

namespace V2Ray.Api.Entity
{
    public class Role : FullAuditEntity<int>, ISoftDelete
    {
        public string Title { get; set; }

        public ICollection<UserRole> Users { get; set; }
        public bool IsDeleted { get; set; }
    }
}