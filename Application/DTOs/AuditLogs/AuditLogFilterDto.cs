using Application.DTOs.Abstract;
using Shared.Enums;

namespace Application.DTOs.AuditLogs
{
    public class AuditLogFilterDto : BasePagingFilterDto
    {
        public int? UserId { get; set; }

        public static readonly string[] AllowedSortFields = 
        {
            "AuditId", "UserId", "Action", "OldValue", "NewValue", "IpAddress", "CreatedAt", "Flag"
        };

        public override Dictionary<string, FieldType> GetFieldTypeMappings() => new()
        {
            ["AuditId"] = FieldType.Numeric,
            ["UserId"] = FieldType.Numeric,
            ["Action"] = FieldType.String,
            ["OldValue"] = FieldType.String,
            ["NewValue"] = FieldType.String,
            ["IpAddress"] = FieldType.String,
            ["Flag"] = FieldType.String,
            ["Field1"] = FieldType.String,
            ["Field2"] = FieldType.String,
            ["Field3"] = FieldType.String,
            ["Field4"] = FieldType.String,
            ["Field5"] = FieldType.String,
            ["CreatedAt"] = FieldType.Date
        };
    }
}
