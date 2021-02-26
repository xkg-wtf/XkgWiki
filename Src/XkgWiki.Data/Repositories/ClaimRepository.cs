using XkgWiki.Domain;

namespace XkgWiki.Data.Repositories
{
    public interface IClaimRepository : IEntityRepository<int, Claim>
    {
    }

    public class ClaimRepository : EntityRepositoryBase<int, Claim>, IClaimRepository
    {
        public ClaimRepository(XkgDbContext dbContext) : base(dbContext)
        {
        }
    }
}