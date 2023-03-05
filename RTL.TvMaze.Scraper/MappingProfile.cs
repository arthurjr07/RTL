using AutoMapper;
using RTL.TvMaze.Scraper.Infrastructure.Entities;
using RTL.TvMaze.Scraper.Models;

namespace RTL.TvMaze.Scraper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Show, ShowModel>()
                .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.Casts.Select(c => c.Person)));
            CreateMap<Person, PersonModel>()
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Birthday.GetValueOrDefault().ToString("yyyy-MM-dd")));
        }
    }
}
