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

        public static readonly string[] AllowedSortFields =
        {
            "LoginId", "UserId", "IsSuccess", "IpAddress", "UserAgent", "Message", "CreatedAt"
        };

        public override Dictionary<string, FieldType> GetFieldTypeMappings() => new()
        {
            ["UserId"] = FieldType.Numeric,
            ["IsSuccess"] = FieldType.Boolean,
            ["IpAddress"] = FieldType.String,
            ["UserAgent"] = FieldType.String,
            ["Message"] = FieldType.String,
            ["CreatedAt"] = FieldType.Date
        };
    }
}
