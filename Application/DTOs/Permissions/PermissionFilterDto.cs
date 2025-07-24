using Application.DTOs.Abstract;
using Shared.Enums;

namespace Application.DTOs.Permissions
{
    public class PermissionFilterDto : BasePagingFilterDto
    {
        public int? PermissionId { get; set; }
        public string? Name { get; set; }
        public string? Module { get; set; }
        public string? Action { get; set; }
        public string? Flag { get; set; }

        public static readonly string[] AllowedSortFields = {
            "PermissionId", "Name", "Module", "Action", "CreatedAt"
        };

        public override Dictionary<string, FieldType> GetFieldTypeMappings() => new()
        {
            ["PermissionId"] = FieldType.Numeric,
            ["Name"] = FieldType.String,
            ["Module"] = FieldType.String,
            ["Action"] = FieldType.String,
            ["Flag"] = FieldType.String
        };
    }
}
