using RTL.TvMaze.Scraper.Infrastructure.Entities;
using System.Linq.Expressions;

namespace RTL.TvMaze.Scraper.Infrastructure.Repositories
{
    public interface IShowAggregateRepository
    {
        Task<int> GetTotalCount();
        Task<Show> GetById(int showId);
        Task<IEnumerable<Show>> GetAll(int pageNumber, int pageSize);
    }
}