using Application.DTOs.Roles;
using Domain.Entities.AccessControl;

namespace Application.Mappings
{
    public static class UserRoleMapper
    {
        public static (int UserId, RoleDto Role) MapToDto((int UserId, Role Role) tuple)
        {
            return (tuple.UserId, RoleMapper.MapToDto(tuple.Role));
        }

        public static IEnumerable<(int UserId, RoleDto Role)> MapToDtoList(IEnumerable<(int UserId, Role Role)> list)
        {
            return list.Select(MapToDto);
        }
    }
}
