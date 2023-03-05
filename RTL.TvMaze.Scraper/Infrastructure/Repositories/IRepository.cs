using System.Linq.Expressions;

namespace RTL.TvMaze.Scraper.Infrastructure.Repositories
{
    public interface IRepository<TEntity> where TEntity : class, new()
    {
        Task<TEntity> AddAsync(TEntity entity);

        Task UpdateAsync(int id, TEntity entity);

        Task AddOrUpdateAsync(int id, TEntity entity);

        Task DeleteAsync(int id);

        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FindByIdAsync(int id);
    }
}
