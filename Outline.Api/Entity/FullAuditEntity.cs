using System;

namespace Outline.Api.Entity
{
    public class FullAuditEntity<TKey> : AuditEntity<TKey> where TKey : IEquatable<TKey>
    {
        public DateTime? UpdateAt { get; set; }

        public int? CreatorUserId { get; set; }

        public int? UpdaterUserId { get; set; }
    }

    public interface ISoftDelete
    {
        public bool IsDeleted { get; set; }
    }
}