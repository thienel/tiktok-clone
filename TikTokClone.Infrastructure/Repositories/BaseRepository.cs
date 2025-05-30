using System.Linq.Expressions;
using TikTokClone.Application.Interfaces.Repositories;

namespace TikTokClone.Infrastructure.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        public Task<TEntity?> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }
        public Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
        public Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }
        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }
        public void Remove(TEntity entity)
        {
            throw new NotImplementedException();
        }
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }
        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
