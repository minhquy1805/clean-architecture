using Application.DTOs.Abstract;
using Shared.Enums;

namespace Application.DTOs.LoginHistories
{
    public class LoginHistoryFilterDto : BasePagingFilterDto
    {
        public int? UserId { get; set; }
        public bool? IsSuccess { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Message { get; set; }
        public string? Flag { get; set; } = "T"; // 👈 dùng để lọc các bản ghi đã bị xóa mềm

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public static readonly string[] AllowedSortFields =
        {
            "UserId", "IsSuccess", "IpAddress", "UserAgent", "Message", "CreatedAt"
        };
    }
}
