using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Application.Interfaces.Services;
using Application.Filters;
using Application.DTOs.LoginHistories;

namespace CommercialNews.Controllers.Admin
{
    [Route("api/v1/admin/login-history")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminLoginHistoryController : BaseController
    {
        private readonly ILoginHistoryService _loginHistoryService;

        public AdminLoginHistoryController(ILoginHistoryService loginHistoryService)
        {
            _loginHistoryService = loginHistoryService;
        }

        /// <summary>
        /// ✅ Get login history by user id
        /// </summary>
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var logs = await _loginHistoryService.GetByUserIdAsync(userId);
            return OkResponse(logs, "Fetched login history by user id.");
        }

        /// <summary>
        /// ✅ Get login history with paging
        /// </summary>
        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] LoginHistoryFilterDto filter)
        {
            
            // Lấy dữ liệu và tổng số bản ghi
            var (data, totalRecords) = await _loginHistoryService.GetPagingAsync(filter);

            // Trả về kết quả kèm thông tin phân trang
            var pagination = new
            {
                currentPage = filter.CurrentPage,
                totalPages = PaginationHelper.CalculateTotalPages(totalRecords, filter.NumberOfRows),
                totalRecords,
                rowsPerPage = filter.NumberOfRows,
                pagesToShow = GridConfig.NumberOfPagesToShow
            };

            return Ok(new { data, pagination });
        }
    }
}
