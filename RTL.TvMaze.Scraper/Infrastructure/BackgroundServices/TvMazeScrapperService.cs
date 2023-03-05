using RTL.TvMaze.Scraper.Infrastructure.Data;
using RTL.TvMaze.Scraper.Infrastructure.Dto;
using RTL.TvMaze.Scraper.Infrastructure.Entities;
using RTL.TvMaze.Scraper.Infrastructure.Repositories;
using RTL.TvMaze.Scraper.Infrastructure.Utilities;
using System.Text.Json;

namespace RTL.TvMaze.Scraper.Infrastructure.BackgroundServices
{
    public class TvMazeScrapperService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private Timer? timer = null;

        private readonly IConfiguration config;
        private readonly IHttpClientFactory httpFactory;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<TvMazeScrapperService> logger;
        

        public TvMazeScrapperService(
            IServiceScopeFactory scopeFactory,
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<TvMazeScrapperService> logger)
        {
            this.logger = logger;
            this.config = config;
            this.httpFactory = httpClientFactory;
            this.scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Scrapper Service running.");
            var interval = config.GetValue<int>("ScrapperService:TimerIntervalInHours");
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(interval));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);
            logger.LogInformation("Scrapper Service is working. Count: {Count}", count);

            var pageNo = 0;
            var hasCompleted = false;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            options.Converters.Add(new DateTimeConverter("yyyy-MM-dd"));

            while (!hasCompleted)
            {
                var httpClient = httpFactory.CreateClient();
                httpClient.BaseAddress = new Uri(config.GetValue<string>("ScrapperService:BaseUrl"));
                var httpResponseMessage = await httpClient.GetAsync($"shows?page={pageNo}");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var showsStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    var shows = await JsonSerializer.DeserializeAsync<IEnumerable<Show>>(showsStream, options);

                    foreach (var show in shows)
                    {
                        Console.WriteLine($"Importing show - {show.Id}");
                        var response = await httpClient.GetAsync($"shows/{show.Id}/cast");
                        using var castsStream = await response.Content.ReadAsStreamAsync();
                        var casts = await JsonSerializer.DeserializeAsync<IEnumerable<Cast>>(castsStream, options);

                        using (var scope = scopeFactory.CreateScope())
                        {
                            try
                            {
                                var dbContext = scope.ServiceProvider.GetRequiredService<ShowDbContext>();
                                var showRepository = new BaseRepository<Show>(dbContext);
                                var personRepository = new BaseRepository<Person>(dbContext);
                                var castRepository = new BaseRepository<ShowCast>(dbContext);

                                await showRepository.AddOrUpdateAsync(show.Id, show);
                                foreach (var person in casts.Select(c => c.Person))
                                {
                                    await personRepository.AddOrUpdateAsync(person.Id, person);
                                    var showCastId = person.Updated; // this is unique.
                                    await castRepository.AddOrUpdateAsync(showCastId, new ShowCast() { Id = showCastId, ShowId = show.Id, PersonId = person.Id });
                                }
                                dbContext.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, $"Unable to save show: {show.Id}");
                            }
                        }
                        await Task.Delay(1000 * 2); // pause for 2 seconds.
                    }
                    pageNo++;
                }
                else
                {
                    if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        hasCompleted = true;
                    }
                }

            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Scrapper Service is stopping.");
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
