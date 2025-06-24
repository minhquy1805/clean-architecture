using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CommercialNews.Responses;


namespace CommercialNews.Controllers.Admin
{
    [Route("api/v1/admin/audit")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminAuditController : BaseController
    {
        private readonly IUserAuditRepository _userAuditRepo;

        public AdminAuditController(IUserAuditRepository userAuditRepo)
        {
            _userAuditRepo = userAuditRepo;
        }

        /// <summary>
        /// ✅ Get audit log by user id
        /// </summary>
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var audits = await _userAuditRepo.GetByUserIdAsync(userId);
            return OkResponse(audits, "Fetched audit log by user id.");
        }

        /// <summary>
        /// ✅ Get audit log with paging
        /// </summary>
        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging(
            [FromQuery] int? userId,
            [FromQuery] int start = 0,
            [FromQuery] int numberOfRows = 10)
        {
            var audits = await _userAuditRepo.GetPagingAsync(userId, start, numberOfRows);
            return OkResponse(audits, "Fetched audit log with paging.");
        }
    }
}
