using System.Collections.Generic;

namespace Outline.Api.Entity
{
    public class Role : Entity<int>
    {
        public string Title { get; set; }

        public ICollection<UserRole> Users { get; set; }

    }
}