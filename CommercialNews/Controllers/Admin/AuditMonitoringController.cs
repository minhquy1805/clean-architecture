using Application.Interfaces.Redis.Tracking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommercialNews.Controllers.Admin
{
    [Route("api/v1/audit-monitoring")]
    [ApiController]
    [Authorize(Policy = "AuditLog:View")]
    public class AuditMonitoringController : BaseController
    {
        private readonly IAuditZSetTrackerService _tracker;

        public AuditMonitoringController(IAuditZSetTrackerService tracker)
        {
            _tracker = tracker;
        }

        /// <summary>
        /// 📊 Lấy top hành động audit được gọi nhiều nhất
        /// </summary>
        [HttpGet("top-actions")]
        public async Task<IActionResult> GetTopActions([FromQuery] int top = 10, [FromQuery] bool byDate = false)
        {
            var results = await _tracker.GetTopActionsAsync(top, useDateSuffix: byDate);
            var data = results.Select(r => new { action = r.Action, count = r.Count });
            return OkResponse(data, "Top audit actions fetched successfully.");
        }

        /// <summary>
        /// 🧮 Đếm tổng số hành động khác nhau đã được track
        /// </summary>
        [HttpGet("count")]
        public async Task<IActionResult> CountTrackedActions([FromQuery] bool byDate = false)
        {
            var count = await _tracker.CountActionsAsync(byDate);
            return OkResponse(count, "Tracked actions counted.");
        }

        /// <summary>
        /// 🗑 Xoá toàn bộ ZSet hành động audit
        /// </summary>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearAll([FromQuery] bool byDate = false)
        {
            var result = await _tracker.ClearAllAsync(byDate);
            if (result)
                return OkResponse(true, "Audit ZSet cleared.");
            else
                return BadRequestResponse("No actions to clear or failed to clear.");
        }
    }
}
