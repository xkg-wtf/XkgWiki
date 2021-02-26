using System;
using Att.Domain;
using Att.Domain.Shared;
using Microsoft.AspNetCore.Identity;

namespace XkgWiki.Domain
{
    public class Claim : IdentityUserClaim<Guid>, IEntity<int>
    {
        public User User { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public bool IsNew()
        {
            return Id == 0;
        }
    }
}