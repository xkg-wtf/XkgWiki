using System;
using System.ComponentModel.DataAnnotations;

namespace Att.Domain.Shared
{
    public interface IEntity<TKey> where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        bool IsNew();
    }

    public interface IEntity : IEntity<Guid>
    {
    }

    public abstract class EntityBase : IEntity
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public bool IsNew()
        {
            return Id == Guid.Empty;
        }
    }
}