namespace RTL.TvMaze.Scraper.Infrastructure.Entities
{
    public class ShowCast : BaseEntity
    {
        public int ShowId { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
