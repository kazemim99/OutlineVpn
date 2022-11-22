using System;
using System.ComponentModel.DataAnnotations;

namespace Outline.Api.Entity
{
    public class Entity<TKey> where TKey : IEquatable<TKey>

    {
        [Key]
        public TKey Id { get; set; }
    }
}