using System;
using Att.Domain.Shared;
using XkgWiki.Domain;

namespace Att.Domain
{
    public class UserRole : IEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public bool IsNew()
        {
            return Id == Guid.Empty;
        }
    }
}