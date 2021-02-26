using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XkgWiki.Domain;

namespace XkgWiki.Data.Repositories
{
    public interface IUserRepository : IEntityRepository<Guid, User>
    {
        Task<User> GetBySubjectIdAsync(string subjectId, bool throwErrorIfNotFound = true);
        Task<User> GetByEmailAsync(string email, bool throwErrorIfNotFound = true);
        Task<TResult> GetByEmailAsync<TResult>(string email, Expression<Func<User, TResult>> selector, bool throwErrorIfNotFound = true);
        Task<User> GetByUsernameAsync(string normalizedUsername, bool throwErrorIfNotFound = false);
        Task<TResult> GetByUsernameAsync<TResult>(string normalizedUsername, Expression<Func<User, TResult>> selector, bool throwErrorIfNotFound = true);
    }

    public class UserRepository : EntityRepositoryBase<Guid, User>, IUserRepository
    {
        public UserRepository(XkgDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User> GetBySubjectIdAsync(string subjectId, bool throwErrorIfNotFound = true)
        {
            var id = Guid.Parse(subjectId);
            return await GetByIdAsync(id, throwErrorIfNotFound);
        }

        public async Task<User> GetByEmailAsync(string email, bool throwErrorIfNotFound = true)
        {
            if (throwErrorIfNotFound)
                return await SingleAsync(u => u.Email == email);

            return await SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<TResult> GetByEmailAsync<TResult>(string email, Expression<Func<User, TResult>> selector, bool throwErrorIfNotFound = true)
        {
            if (throwErrorIfNotFound)
                return await SingleAsync(u => u.Email == email, selector);

            return await SingleOrDefaultAsync(u => u.Email == email, selector);
        }

        public async Task<User> GetByUsernameAsync(string normalizedUsername, bool throwErrorIfNotFound = false)
        {
            if (throwErrorIfNotFound)
                return await SingleAsync(u => u.NormalizedUserName == normalizedUsername || u.UserName == normalizedUsername);

            return await SingleOrDefaultAsync(u => u.NormalizedUserName == normalizedUsername || u.UserName == normalizedUsername);
        }

        public async Task<TResult> GetByUsernameAsync<TResult>(string normalizedUsername, Expression<Func<User, TResult>> selector,
            bool throwErrorIfNotFound = true)
        {
            if (throwErrorIfNotFound)
                return await SingleAsync(u => u.NormalizedUserName == normalizedUsername || u.UserName == normalizedUsername, selector);

            return await SingleOrDefaultAsync(u => u.NormalizedUserName == normalizedUsername || u.UserName == normalizedUsername, selector);
        }
    }
}