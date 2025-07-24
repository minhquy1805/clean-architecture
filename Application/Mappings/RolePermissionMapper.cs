using Application.DTOs.Permissions;
using Domain.Entities.AccessControl;

namespace Application.Mappings
{
    public static class RolePermissionMapper
    {
        public static (int RoleId, PermissionDto Permission) MapToDto((int RoleId, Permission Permission) tuple)
        {
            return (tuple.RoleId, PermissionMapper.MapToDto(tuple.Permission));
        }

        public static IEnumerable<(int RoleId, PermissionDto Permission)> MapToDtoList(IEnumerable<(int RoleId, Permission Permission)> list)
        {
            return list.Select(MapToDto);
        }
    }
}
