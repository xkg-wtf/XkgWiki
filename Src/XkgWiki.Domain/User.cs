using System;
using Att.Domain.Shared;
using Microsoft.AspNetCore.Identity;

namespace XkgWiki.Domain
{
    public class User : IdentityUser<Guid>, IEntity
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public bool IsNew()
        {
            return Id == Guid.Empty;
        }
    }
}