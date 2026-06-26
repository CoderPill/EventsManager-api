using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EventsManager.Infrastructure.Persistence.Common.Repository
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
     where TEntity : BaseEntity
    {
        protected readonly DbContext _DbContext;
        protected DbSet<TEntity> _DbSet;
        public BaseRepository(DbContext dbContext)
        {
            _DbContext = dbContext;
            _DbSet = dbContext.Set<TEntity>();
        }

        public async Task SaveChangesAsync()
        {
            await _DbContext.SaveChangesAsync();
        }
        public virtual async Task AddAsync(TEntity entity)
        {
            if (entity.CreateDate == default)
                entity.CreateDate = DateTime.Now;
            await _DbSet.AddAsync(entity);
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _DbSet.FindAsync(id);
        }
        public async Task<List<TEntity>> GetAllAsync(
             Expression<Func<TEntity, bool>>? predicate = null
            , bool noTracking = true
            , params string[] includes)
        {
            return await BuildQuery(predicate, noTracking,includes).ToListAsync();
        }

        public async Task<TEntity?> GetFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate
            , bool noTracking = true
            , params string[] includes)
        {
            return await BuildQuery(predicate, noTracking, includes).FirstOrDefaultAsync();
        }

        protected IQueryable<TEntity> BuildQuery(
            Expression<Func<TEntity, bool>>? predicate = null
            ,bool noTracking = true
            ,params string[] includes)
        {
            IQueryable<TEntity> query = _DbSet;

            if (noTracking)
            {
                query = query.AsNoTrackingWithIdentityResolution();
            }
            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }
        public void Update(TEntity entity)
        {
            _DbContext.Update(entity);
        }

        public void UpdateCollection(IEnumerable<TEntity> entities)
        {
            _DbContext.UpdateRange(entities);
        }

        public void Delete(TEntity entity)
        {
            _DbContext.Remove(entity);
        }

        public void DeleteCollection(IEnumerable<TEntity> entities)
        {
            _DbContext.RemoveRange(entities);
        }
    }
}
