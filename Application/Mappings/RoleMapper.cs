using Application.DTOs.Roles;
using Domain.Entities.AccessControl;

namespace Application.Mappings
{
    public static class RoleMapper
    {
        public static RoleDto MapToDto(Role entity)
        {
            return new RoleDto
            {
                RoleId = entity.RoleId,
                RoleName = entity.RoleName,
                Description = entity.Description,
                Flag = entity.Flag,
                CreatedAt = entity.CreatedAt
            };
        }

        public static Role MapToEntity(RoleDto dto)
        {
            return new Role
            {
                RoleId = dto.RoleId,
                RoleName = dto.RoleName,
                Description = dto.Description,
                Flag = dto.Flag,
                CreatedAt = dto.CreatedAt
            };
        }

        public static int GetDtoId(RoleDto dto) => dto.RoleId;
    }
}
