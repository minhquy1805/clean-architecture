using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Application.Interfaces.Services;
using Application.DTOs.LoginHistories;

namespace CommercialNews.Controllers.Admin
{
    [Authorize(Policy = "Permission:LoginHistory.View")]
    [Route("api/v1/admin/login-history")]
    [ApiController]
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
            return OkResponse(logs, "Fetched login history by user ID.");
        }

        /// <summary>
        /// ✅ Get login history with paging
        /// </summary>
        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] LoginHistoryFilterDto filter)
        {
            var (data, totalRecords) = await _loginHistoryService.GetPagingAsync(filter);

            var pagination = new
            {
                currentPage = filter.CurrentPage,
                totalPages = PaginationHelper.CalculateTotalPages(totalRecords, filter.NumberOfRows),
                totalRecords,
                rowsPerPage = filter.NumberOfRows,
                pagesToShow = GridConfig.NumberOfPagesToShow
            };

            return OkResponse(new { data, pagination }, "Fetched login history with paging.");
        }

        /// <summary>
        /// ✅ Get latest login of a user
        /// </summary>
        [HttpGet("last-login/{userId}")]
        public async Task<IActionResult> GetLastLogin(int userId)
        {
            var lastLogin = await _loginHistoryService.GetLastLoginAsync(userId);
            return OkResponse(lastLogin, "Fetched last login history.");
        }
    }
}

