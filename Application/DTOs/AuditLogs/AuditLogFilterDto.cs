using Application.DTOs.Abstract;

namespace Application.DTOs.AuditLogs
{
    public class AuditLogFilterDto : BasePagingFilterDto
    {
        public int? UserId { get; set; }
        public string? Action { get; set; }
        public string? Flag { get; set; }
        public DateTime? FromCreatedAt { get; set; }
        public DateTime? ToCreatedAt { get; set; }

        // ✅ Dùng cho Mongo nếu bạn cần validate sort fields
        public static readonly string[] AllowedSortFields =
        {
            "UserId", "Action", "CreatedAt", "Flag"
        };
    }
}
