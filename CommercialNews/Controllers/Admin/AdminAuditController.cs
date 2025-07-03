using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Application.Interfaces.Services;
using Application.DTOs.AuditLogs;


namespace CommercialNews.Controllers.Admin
{
    [Route("api/v1/admin/audit")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminAuditController : BaseController
    {
        private readonly IAuditService _auditService;

        public AdminAuditController(IAuditService auditService)
        {
            _auditService = auditService;
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
    }
}
