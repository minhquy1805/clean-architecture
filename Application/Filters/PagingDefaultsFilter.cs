using Application.DTOs.Abstract;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Helpers;


namespace Application.Filters
{
    public class PagingDefaultsFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is BasePagingFilterDto dto)
                {
                    // Áp dụng mặc định nếu chưa hợp lệ
                    if (dto.NumberOfRows <= 0)
                        dto.NumberOfRows = GridConfig.NumberOfRows;

                    if (dto.CurrentPage <= 0)
                        dto.CurrentPage = 1;

                    if (string.IsNullOrWhiteSpace(dto.SortBy))
                        dto.SortBy = "CreatedAt";

                    if (string.IsNullOrWhiteSpace(dto.SortDirection))
                        dto.SortDirection = "DESC";
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Không cần xử lý gì sau action
        }
    }
}
