using System;
using Att.Domain.Shared;
using Microsoft.AspNetCore.Identity;

namespace Att.Domain
{
    public class Role : IdentityRole<Guid>, IEntity<Guid>
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public bool IsNew()
        {
            return Id == Guid.Empty;
        }
    }
}