using Microsoft.EntityFrameworkCore;
using RTL.TvMaze.Scraper.Infrastructure.Entities;

namespace RTL.TvMaze.Scraper.Infrastructure.Data
{
    public class ShowDbContext : DbContext
    {
        public ShowDbContext(DbContextOptions<ShowDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Show>().ToTable("Show").HasKey(u => u.Id);
            modelBuilder.Entity<Person>().ToTable("Person").HasKey(u => u.Id);
            modelBuilder.Entity<ShowCast>().ToTable("ShowCast").HasKey(u => u.Id);

            modelBuilder.Entity<Show>().HasMany<ShowCast>(sc => sc.Casts);
            modelBuilder.Entity<ShowCast>().HasOne<Person>(sc => sc.Person);
        }

        public DbSet<Show> Shows { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<ShowCast> ShowCasts { get; set; }
    }
}
