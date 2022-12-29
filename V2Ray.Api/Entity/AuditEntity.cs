using System;

namespace V2Ray.Api.Entity
{
    public class AuditEntity<TKey> : Entity<TKey> where TKey : IEquatable<TKey>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}