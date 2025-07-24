using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Application.Interfaces.Services;
using Application.DTOs.AuditLogs;
using Application.Interfaces.Redis.Caching;

namespace CommercialNews.Controllers.Admin
{
    [Authorize(Policy = "AuditLog:View")]
    [Route("api/v1/admin/audit")]
    [ApiController]
    public class AdminAuditController : BaseController
    {
        private readonly IAuditService _auditService;
        private readonly IUserAuditCacheService _auditCache; // ✅ Inject thêm

        public AdminAuditController(
            IAuditService auditService,
            IUserAuditCacheService auditCache // ✅ Inject thêm
        )
        {
            _auditService = auditService;
            _auditCache = auditCache;
        }

        /// <summary>
        /// ✅ Get audit log by user id
        /// </summary>
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var audits = await _auditService.GetByUserIdAsync(userId);
            return OkResponse(audits, "Fetched audit log by user id.");
        }

        /// <summary>
        /// ✅ Get audit log with paging
        /// </summary>
        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] AuditLogFilterDto filter)
        {
            var (data, totalRecords) = await _auditService.GetPagingAsync(filter);

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

        /// <summary>
        /// ✅ Get last audit info (from Redis)
        /// </summary>
        [HttpGet("last/{userId}")]
        public async Task<IActionResult> GetLastAuditInfo(int userId)
        {
            var time = await _auditCache.GetLastAuditTimeAsync(userId);
            var action = await _auditCache.GetLastAuditActionAsync(userId);

            if (time == null || string.IsNullOrWhiteSpace(action))
                return NotFoundResponse("No audit data found in cache.");

            return OkResponse(new
            {
                userId,
                lastAction = action,
                lastTime = time
            }, "Fetched last audit info from Redis cache.");
        }
    }
}