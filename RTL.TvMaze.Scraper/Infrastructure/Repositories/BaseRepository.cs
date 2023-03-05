using Microsoft.EntityFrameworkCore;
using RTL.TvMaze.Scraper.Infrastructure.Data;
using RTL.TvMaze.Scraper.Infrastructure.Entities;
using RTL.TvMaze.Scraper.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace RTL.TvMaze.Scraper.Infrastructure.Repositories
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly ShowDbContext context;
        protected readonly DbSet<TEntity> DbSet;

        public BaseRepository(ShowDbContext context)
        {
            this.context = context;
            DbSet = context.Set<TEntity>();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var result = await DbSet.AddAsync(entity);
            return result.Entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await DbSet.FindAsync(id).ConfigureAwait(false);
            if (entity != null)
            {
                DbSet.Remove(entity);
            }
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var result = DbSet.Where(predicate);
            return await result.ToListAsync().ConfigureAwait(false);
        }

        public async Task<TEntity> FindByIdAsync(int id)
        {
            return await DbSet.FindAsync(id).ConfigureAwait(false);
        }

        public async Task UpdateAsync(int id, TEntity updated)
        {
            var entity = await DbSet.FindAsync(id).ConfigureAwait(false);
            if (entity != null)
            {
                DbSet.Update(updated);
            }
        }

        public async Task AddOrUpdateAsync(int id, TEntity updated)
        {
            var entity = await DbSet.FindAsync(id).ConfigureAwait(false);
            if (entity != null)
            {
                if (context.Entry(entity).State == EntityState.Unchanged)
                {
                    context.Entry(entity).CurrentValues.SetValues(updated);
                    DbSet.Update(entity);
                }
            }
            else
            {
                DbSet.Add(updated);
            }
        }
    }
}
