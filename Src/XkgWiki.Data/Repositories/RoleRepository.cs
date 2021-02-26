using System;
using Att.Domain;

namespace XkgWiki.Data.Repositories
{
    public interface IRoleRepository : IEntityRepository<Guid, Role>
    {
    }

    public class RoleRepository : EntityRepositoryBase<Guid, Role>, IRoleRepository
    {
        public RoleRepository(XkgDbContext dbContext) : base(dbContext)
        {
        }
    }
}