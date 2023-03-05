namespace RTL.TvMaze.Scraper.Models
{
    public class ShowModel
    {
        public int  Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<PersonModel> Cast { get; set; }
    }
}
