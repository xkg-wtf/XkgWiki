using System;
using Att.Domain;

namespace XkgWiki.Data.Repositories
{
    public interface IUserTokenRepository : IEntityRepository<Guid, UserToken>
    {
    }

    public class UserTokenRepository : EntityRepositoryBase<Guid, UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(XkgDbContext dbContext) : base(dbContext)
        {
        }
    }
}