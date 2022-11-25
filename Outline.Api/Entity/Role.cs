using System.Collections.Generic;

namespace Outline.Api.Entity
{
    public class Role : FullAuditEntity<int>,ISoftDelete
    {
        public string Title { get; set; }

        public ICollection<UserRole> Users { get; set; }
        public bool IsDeleted { get ; set; }
    }
}