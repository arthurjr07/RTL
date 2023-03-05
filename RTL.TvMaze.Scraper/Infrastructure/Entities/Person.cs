using System.ComponentModel.DataAnnotations.Schema;

namespace RTL.TvMaze.Scraper.Infrastructure.Entities
{
    public class Person : BaseEntity
    {
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }

        [NotMapped]
        public int Updated { get; set; }
    }
}
