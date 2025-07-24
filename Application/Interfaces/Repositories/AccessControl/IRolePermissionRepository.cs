using Domain.Entities.AccessControl;

namespace Application.Interfaces.Repositories.AccessControl
{
    public interface IRolePermissionRepository
    {
        Task InsertAsync(int roleId, int permissionId);
        Task DeleteAsync(int roleId, int permissionId);
        Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId);
        Task<IEnumerable<int>> GetPermissionIdsByRoleIdAsync(int roleId); // tiện để check trong middleware

        Task<IEnumerable<Permission>> GetPermissionsByRoleIdsAsync(IEnumerable<int> roleIds);
    }
}
