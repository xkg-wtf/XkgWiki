using System;
using Att.Domain.Shared;
using Microsoft.AspNetCore.Identity;

namespace Att.Domain
{
    public class UserToken : IdentityUserToken<Guid>, IEntity
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