using System;
using Att.Domain;

namespace XkgWiki.Data.Repositories
{
    public interface IUserRoleRepository : IEntityRepository<Guid, UserRole>
    {
    }

    public class UserRoleRepository : EntityRepositoryBase<Guid, UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(XkgDbContext dbContext) : base(dbContext)
        {
        }
    }
}