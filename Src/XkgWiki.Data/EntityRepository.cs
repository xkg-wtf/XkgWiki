﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Att.Domain.Shared;
using Att.Shared;
using Microsoft.EntityFrameworkCore;

namespace XkgWiki.Data
{
    public interface IEntityRepository<in TKey, TEntity>
        where TKey : struct, IEquatable<TKey>, IComparable<TKey>
        where TEntity : class, IEntity<TKey>
    {
        void Save(TEntity entity);

        void Delete(TEntity entity);

        Task<TEntity> GetByIdAsync(TKey id, bool throwExceptionIfNotFound = true);

        Task<TResult> GetByIdAsync<TResult>(TKey id, Expression<Func<TEntity, TResult>> selector, bool throwExceptionIfNotFound = true);

        IQueryable<TEntity> QueryAll();

        IQueryable<TResult> QueryAll<TResult>(Expression<Func<TEntity, TResult>> selector);

        IQueryable<TEntity> QueryMany(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TResult> QueryMany<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);

        Task<IReadOnlyCollection<TEntity>> GetAllAsync();

        Task<IReadOnlyCollection<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

        Task<IReadOnlyCollection<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IReadOnlyCollection<TResult>> GetManyAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);

        Task<TEntity> FirstOrDefaultAsync();

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> where);

        Task<TEntity> FirstAsync();

        Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> where);

        Task<TEntity> SingleOrDefaultAsync();

        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> where);

        Task<TEntity> SingleAsync();

        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> where);

        Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

        Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TResult>> selector);

        Task<TResult> FirstAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

        Task<TResult> FirstAsync<TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TResult>> selector);

        Task<TResult> SingleOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

        Task<TResult> SingleOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TResult>> selector);

        Task<TResult> SingleAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

        Task<TResult> SingleAsync<TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TResult>> selector);

        Task<bool> AnyAsync();

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync();

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }

    public abstract class EntityRepositoryBase<TKey, TEntity> : IEntityRepository<TKey, TEntity>
        where TKey : struct, IEquatable<TKey>, IComparable<TKey>
        where TEntity : class, IEntity<TKey>
    {
        private XkgDbContext DbContext { get; }
        private DbSet<TEntity> DbSet { get; }

        protected EntityRepositoryBase(XkgDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<TEntity>();
        }

        public virtual void Save(TEntity entity)
        {
            if (entity.IsNew() && entity.CreatedAt.IsEmpty())
            {
                entity.CreatedAt = DateTimeOffset.UtcNow;
            }
            else if (!entity.IsNew())
            {
                var entityEntry = DbContext.Entry(entity);
                entity.UpdatedAt = entityEntry.State switch
                {
                    EntityState.Modified when !entityEntry.Property(nameof(EntityBase.UpdatedAt)).IsModified => DateTimeOffset.UtcNow,
                    EntityState.Detached => throw new InvalidOperationException("Can't save detached entity."),
                    _ => entity.UpdatedAt,
                };
            }

            DbSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public async Task<TEntity> GetByIdAsync(TKey id, bool throwExceptionIfNotFound = true)
        {
            var result = await DbSet.FindAsync(id);
            if (result == null && throwExceptionIfNotFound)
                ThrowEntityNotFoundException(id);

            return result;
        }

        public Task<TResult> GetByIdAsync<TResult>(TKey id, Expression<Func<TEntity, TResult>> selector, bool throwExceptionIfNotFound = true)
        {
            return SingleAsync(x => x.Id.Equals(id), selector);
        }

        public IQueryable<TEntity> QueryAll()
        {
            return DbSet;
        }

        public IQueryable<TResult> QueryAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return QueryAll().Select(selector);
        }

        public IQueryable<TEntity> QueryMany(Expression<Func<TEntity, bool>> predicate)
        {
            return QueryAll().Where(predicate);
        }

        public IQueryable<TResult> QueryMany<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return QueryMany(predicate).Select(selector);
        }

        public async Task<IReadOnlyCollection<TEntity>> GetAllAsync()
        {
            return await QueryAll().ToArrayAsync();
        }

        public async Task<IReadOnlyCollection<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return await QueryAll(selector).ToArrayAsync();
        }

        public async Task<IReadOnlyCollection<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await QueryMany(predicate).ToArrayAsync();
        }

        public async Task<IReadOnlyCollection<TResult>> GetManyAsync<TResult>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector)
        {
            return await QueryMany(predicate, selector).ToArrayAsync();
        }

        public async Task<TEntity> FirstOrDefaultAsync()
        {
            return await QueryAll().FirstOrDefaultAsync();
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> where)
        {
            return await QueryAll().FirstOrDefaultAsync(where);
        }

        public async Task<TEntity> FirstAsync()
        {
            return await QueryAll().FirstAsync();
        }

        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> where)
        {
            return await QueryAll().FirstAsync(where);
        }

        public async Task<TEntity> SingleOrDefaultAsync()
        {
            return await QueryAll().SingleOrDefaultAsync();
        }

        public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> where)
        {
            return await QueryAll().SingleOrDefaultAsync(where);
        }

        public async Task<TEntity> SingleAsync()
        {
            return await QueryAll().SingleAsync();
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> where)
        {
            return await QueryAll().SingleAsync(where);
        }

        public async Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return await QueryAll(selector).FirstOrDefaultAsync();
        }

        public async Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TResult>> selector)
        {
            return await QueryMany(where, selector).FirstOrDefaultAsync();
        }

        public async Task<TResult> FirstAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return await QueryAll(selector).FirstAsync();
        }

        public async Task<TResult> FirstAsync<TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TResult>> selector)
        {
            return await QueryMany(where, selector).FirstAsync();
        }

        public async Task<TResult> SingleOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return await QueryAll(selector).SingleOrDefaultAsync();
        }

        public async Task<TResult> SingleOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TResult>> selector)
        {
            return await QueryMany(where, selector).SingleOrDefaultAsync();
        }

        public async Task<TResult> SingleAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return await QueryAll(selector).SingleAsync();
        }

        public async Task<TResult> SingleAsync<TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TResult>> selector)
        {
            return await QueryMany(where, selector).SingleAsync();
        }

        public async Task<bool> AnyAsync()
        {
            return await QueryAll().AnyAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await QueryAll().AnyAsync(predicate);
        }

        public async Task<int> CountAsync()
        {
            return await QueryAll().CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await QueryMany(predicate).CountAsync();
        }

        #region Helpers

        private static void ThrowEntityNotFoundException(TKey entityId)
        {
            throw new InvalidOperationException($"Failed to find entity of type '{typeof(TEntity)}' by id '{entityId}'.");
        }

        #endregion Helpers
    }
}