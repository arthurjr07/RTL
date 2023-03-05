using Microsoft.EntityFrameworkCore;
using RTL.TvMaze.Scraper.Infrastructure.Data;
using RTL.TvMaze.Scraper.Infrastructure.Entities;
using System.Linq.Expressions;

namespace RTL.TvMaze.Scraper.Infrastructure.Repositories
{
    public class ShowAggregateRepository : IShowAggregateRepository
    {
        private readonly ShowDbContext context;
        public ShowAggregateRepository(ShowDbContext context)
        {
            this.context = context;
        }

        public async Task<int> GetTotalCount()
        {
            return await context.Shows.CountAsync();
        }

        public async Task<Show> GetById(int showId)
        {
            return await context.Shows.Include(c => c.Casts).ThenInclude(c => c.Person).FirstOrDefaultAsync(c => c.Id == showId);
        }

        public async Task<IEnumerable<Show>> GetAll(int pageNumber, int pageSize)
        {
            return await context.Shows.Include(c => c.Casts).ThenInclude(c => c.Person)
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
        }
    }
}
