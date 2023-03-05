using FluentValidation;
using RTL.TvMaze.Scraper.Models;

namespace RTL.TvMaze.Scraper.Validations
{
    public class PaginationFilterValidator : AbstractValidator<PaginationFilter> 
    {
        public PaginationFilterValidator()
        {
            RuleFor(p => p.PageNumber).GreaterThan(0).WithMessage("Please ensure page number is greater than 0");
            RuleFor(p => p.PageSize).GreaterThan(0).WithMessage("Please ensure page size is greater than 0");
        }
    }
}
