using Application.DTOs.AuditLogs;
using Domain.Entities;
using Newtonsoft.Json;

namespace Application.Mappings
{
    public static class AuditMapper
    {
        public static UserAuditDto ToDto(UserAudit entity)
        {
            return new UserAuditDto
            {
                AuditId = entity.Id,
                UserId = entity.UserId,
                Action = entity.Action,
                OldValue = entity.OldValue,
                NewValue = entity.NewValue,
                IpAddress = entity.IpAddress,
                Flag = entity.Flag,
                Field1 = entity.Field1,
                Field2 = entity.Field2,
                Field3 = entity.Field3,
                Field4 = entity.Field4,
                Field5 = entity.Field5,
                CreatedAt = entity.CreatedAt,

                OldObject = TryParseJson(entity.OldValue),
                NewObject = TryParseJson(entity.NewValue)
            };
        }

        public static UserAudit ToEntity(UserAuditDto dto)
        {
            return new UserAudit
            {
                Id = dto.AuditId,
                UserId = dto.UserId,
                Action = dto.Action,
                OldValue = dto.OldValue,
                NewValue = dto.NewValue,
                IpAddress = dto.IpAddress,
                Flag = dto.Flag,
                Field1 = dto.Field1,
                Field2 = dto.Field2,
                Field3 = dto.Field3,
                Field4 = dto.Field4,
                Field5 = dto.Field5,
                CreatedAt = dto.CreatedAt
            };
        }

        private static Dictionary<string, object>? TryParseJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;

            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(json!);
            }
            catch
            {
                return null; // fallback nếu JSON không hợp lệ
            }
        }
    }
}
