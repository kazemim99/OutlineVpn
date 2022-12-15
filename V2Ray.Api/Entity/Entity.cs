using System;
using System.ComponentModel.DataAnnotations;

namespace V2Ray.Api.Entity
{
    public class Entity<TKey> where TKey : IEquatable<TKey>

    {
        [Key]
        public TKey Id { get; set; }
    }
}