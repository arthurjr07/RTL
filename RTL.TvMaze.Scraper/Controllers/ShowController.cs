using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RTL.TvMaze.Scraper.Infrastructure.Repositories;
using RTL.TvMaze.Scraper.Infrastructure.Services;
using RTL.TvMaze.Scraper.Models;

namespace RTL.TvMaze.Scraper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowController : ControllerBase
    {
        
        private readonly IMapper mapper;
        private readonly IUriService uriService;
        private readonly IShowAggregateRepository repository;
        private readonly ILogger<ShowController> logger;
        public ShowController(
            IShowAggregateRepository repository,
            IUriService uriService,
            IMapper mapper,
            ILogger<ShowController> logger)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.uriService = uriService;
            this.logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var show = await this.repository.GetById(id);
                var model = this.mapper.Map<ShowModel>(show);
                return Ok(model);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                logger.LogError(ex, message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            try
            {
                var route = $"/api/{this.RouteData.Values["controller"]}";
                var shows = await this.repository.GetAll(filter.PageNumber, filter.PageSize);
                var model = this.mapper.Map<List<ShowModel>>(shows);
                var totalRecords = await this.repository.GetTotalCount();
                var response = new PagedResponse<List<ShowModel>>(model, filter.PageNumber, filter.PageSize);
                var totalPages = ((double)totalRecords / (double)filter.PageSize);
                int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
                response.NextPage =
                    filter.PageNumber >= 1 && filter.PageNumber < roundedTotalPages
                    ? uriService.GetPageUri(new PaginationFilter(filter.PageNumber + 1, filter.PageSize), route)
                    : null;
                response.PreviousPage =
                    filter.PageNumber - 1 >= 1 && filter.PageNumber <= roundedTotalPages
                    ? uriService.GetPageUri(new PaginationFilter(filter.PageNumber - 1, filter.PageSize), route)
                    : null;
                response.FirstPage = uriService.GetPageUri(new PaginationFilter(1, filter.PageSize), route);
                response.LastPage = uriService.GetPageUri(new PaginationFilter(roundedTotalPages, filter.PageSize), route);
                response.TotalPages = roundedTotalPages;
                response.TotalRecords = totalRecords;
                return Ok(response);
            }
            catch(Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message; 
                logger.LogError(ex, message);
                return StatusCode(500);
            }
        }
    }
}
