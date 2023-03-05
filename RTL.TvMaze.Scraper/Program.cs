using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using RTL.TvMaze.Scraper.Infrastructure.BackgroundServices;
using RTL.TvMaze.Scraper.Infrastructure.Data;
using RTL.TvMaze.Scraper.Infrastructure.Entities;
using RTL.TvMaze.Scraper.Infrastructure.Repositories;
using RTL.TvMaze.Scraper.Infrastructure.Services;
using RTL.TvMaze.Scraper.Models;
using RTL.TvMaze.Scraper.Validations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(typeof(Program));

// Add services to the container.
builder.Services.AddDbContext<ShowDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddFluentValidation(options =>
    {
        options.ImplicitlyValidateChildProperties = true;
        options.ImplicitlyValidateRootCollectionElements = true;
        options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<TvMazeScrapperService>();
builder.Services.AddTransient<IRepository<Show>, BaseRepository<Show>>();
builder.Services.AddTransient<IRepository<Person>, BaseRepository<Person>>();
builder.Services.AddTransient<IRepository<ShowCast>, BaseRepository<ShowCast>>();
builder.Services.AddTransient<IShowAggregateRepository, ShowAggregateRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IUriService>(o =>
{
    var accessor = o.GetRequiredService<IHttpContextAccessor>();
    var request = accessor.HttpContext.Request;
    var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
    return new UriService(uri);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
