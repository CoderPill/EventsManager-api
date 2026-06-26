using EventsManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EventsManager.Application.Common.Interfaces.Persistence
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task SaveChangesAsync();
        public Task AddAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(int id);
        Task<List<TEntity>> GetAllAsync(
             Expression<Func<TEntity, bool>>? predicate = null
            , bool noTracking = true
            , params string[] includes);
        Task<TEntity?> GetFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate
            , bool noTracking = true
            , params string[] includes);
        void Update(TEntity entity);
        void UpdateCollection(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void DeleteCollection(IEnumerable<TEntity> entities);
    }
}
