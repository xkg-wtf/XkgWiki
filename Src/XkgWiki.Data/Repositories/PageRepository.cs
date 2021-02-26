using System;
using System.Threading.Tasks;
using XkgWiki.Domain;

namespace XkgWiki.Data.Repositories
{
    public interface IPageRepository : IEntityRepository<Guid, Page>
    {
        Task<Page> GetByUrlAsync(string url);
    }

    public class PageRepository : EntityRepositoryBase<Guid, Page>, IPageRepository
    {
        public PageRepository(XkgDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Page> GetByUrlAsync(string url)
        {
            return await SingleOrDefaultAsync(p => p.Url == url);
        }
    }
}