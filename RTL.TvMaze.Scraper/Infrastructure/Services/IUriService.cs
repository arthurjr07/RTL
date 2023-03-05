using RTL.TvMaze.Scraper.Models;

namespace RTL.TvMaze.Scraper.Infrastructure.Services
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}
