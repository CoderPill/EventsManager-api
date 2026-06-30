using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Core.Common.Time;
using EventsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EventsManager.Infrastructure.Persistence.Common.Repository
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
     where TEntity : BaseEntity
    {
        protected readonly DbContext _dbContext;
        protected DbSet<TEntity> _dbSet;
        protected IDateTimeProvider _timeProvider;
        public BaseRepository(DbContext dbContext, IDateTimeProvider dateTimeProvider)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
            _timeProvider = dateTimeProvider;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        public virtual async Task AddAsync(TEntity entity)
        {
            if (entity.CreationDate == default)
                entity.CreationDate = _timeProvider.GetNowColombia();
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<List<TEntity>> GetAllAsync(
             Expression<Func<TEntity, bool>>? predicate = null
            , bool noTracking = true
            , params string[] includes)
        {
            return await BuildQuery(predicate, noTracking, includes).ToListAsync();
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
            , bool noTracking = true
            , params string[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

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
            _dbContext.Update(entity);
        }

        public void UpdateCollection(IEnumerable<TEntity> entities)
        {
            _dbContext.UpdateRange(entities);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Remove(entity);
        }

        public void DeleteCollection(IEnumerable<TEntity> entities)
        {
            _dbContext.RemoveRange(entities);
        }
    }
}
