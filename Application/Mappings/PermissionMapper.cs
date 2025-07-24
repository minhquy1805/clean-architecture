using Application.DTOs.Permissions;
using Domain.Entities.AccessControl;

namespace Application.Mappings
{
    public static class PermissionMapper
    {
        public static PermissionDto MapToDto(Permission entity)
        {
            return new PermissionDto
            {
                PermissionId = entity.PermissionId,
                Name = entity.Name,
                Module = entity.Module,
                Action = entity.Action,
                Description = entity.Description,
                Flag = entity.Flag,
                CreatedAt = entity.CreatedAt
            };
        }

        public static Permission MapToEntity(PermissionDto dto)
        {
            return new Permission
            {
                PermissionId = dto.PermissionId,
                Name = dto.Name,
                Module = dto.Module,
                Action = dto.Action,
                Description = dto.Description,
                Flag = dto.Flag,
                CreatedAt = dto.CreatedAt
            };
        }

        public static int GetDtoId(PermissionDto dto) => dto.PermissionId;
    }
}
