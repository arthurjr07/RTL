using RTL.TvMaze.Scraper.Infrastructure.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RTL.TvMaze.Scraper.Infrastructure.Entities
{
    public class Show : BaseEntity
    {
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public IList<ShowCast> Casts { get; set; }
    }
}
