using Application.DTOs.Abstract;
using Shared.Enums;

namespace Application.DTOs.Roles
{
    public class RoleFilterDto : BasePagingFilterDto
    {
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Flag { get; set; }

        public static readonly string[] AllowedSortFields = {
            "RoleId", "RoleName", "CreatedAt"
        };

        public override Dictionary<string, FieldType> GetFieldTypeMappings() => new()
        {
            ["RoleId"] = FieldType.Numeric,
            ["RoleName"] = FieldType.String,
            ["Flag"] = FieldType.String
        };
    }
}
