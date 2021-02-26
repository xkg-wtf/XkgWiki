using System;
using Att.Domain;

namespace XkgWiki.Data.Repositories
{
    public interface IUserLoginRepository : IEntityRepository<Guid, UserLogin>
    {
    }

    public class UserLoginRepository : EntityRepositoryBase<Guid, UserLogin>, IUserLoginRepository
    {
        public UserLoginRepository(XkgDbContext dbContext) : base(dbContext)
        {
        }
    }
}