using Application.DTOs.Permissions;

namespace Application.Interfaces.Services.AccessControl
{
    public interface IRolePermissionService
    {
        Task AddPermissionToRoleAsync(int roleId, int permissionId);
        Task RemovePermissionFromRoleAsync(int roleId, int permissionId);
        Task<IEnumerable<PermissionDto>> GetPermissionsByRoleIdAsync(int roleId);
        Task<IEnumerable<int>> GetPermissionIdsByRoleIdAsync(int roleId);

        Task<IEnumerable<PermissionDto>> GetPermissionsByRoleIdsAsync(IEnumerable<int> roleIds);
    }
}
