using Application.DTOs.Abstract;
using Shared.Enums;

namespace Application.DTOs.Users.Filters
{
    public class UserFilterDto : BasePagingFilterDto
    {
        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public bool? IsActive { get; set; }
        public string? Flag { get; set; }

        public static readonly string[] AllowedSortFields = {
            "UserId", "FullName", "Email", "CreatedAt", "UpdatedAt"
        };

        public override Dictionary<string, FieldType> GetFieldTypeMappings() => new()
        {
            ["UserId"] = FieldType.Numeric,
            ["FullName"] = FieldType.String,
            ["Email"] = FieldType.String,
            ["PhoneNumber"] = FieldType.String,
            ["Gender"] = FieldType.String,
            ["IsActive"] = FieldType.Boolean,
            ["Flag"] = FieldType.String
        };
    }
}
