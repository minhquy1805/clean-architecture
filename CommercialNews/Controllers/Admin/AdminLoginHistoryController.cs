using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CommercialNews.Responses;

namespace CommercialNews.Controllers.Admin
{
    [Route("api/v1/admin/login-history")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminLoginHistoryController : BaseController
    {
        private readonly ILoginHistoryRepository _loginHistoryRepo;

        public AdminLoginHistoryController(ILoginHistoryRepository loginHistoryRepo)
        {
            _loginHistoryRepo = loginHistoryRepo;
        }

        /// <summary>
        /// ✅ Get login history by user id
        /// </summary>
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var logs = await _loginHistoryRepo.GetByUserIdAsync(userId);
            return OkResponse(logs, "Fetched login history by user id.");
        }

        /// <summary>
        /// ✅ Get login history with paging
        /// </summary>
        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging(
            [FromQuery] int? userId,
            [FromQuery] int start = 0,
            [FromQuery] int numberOfRows = 10)
        {
            var logs = await _loginHistoryRepo.GetPagingAsync(userId, start, numberOfRows);
            return OkResponse(logs, "Fetched login history with paging.");
        }
    }
}
