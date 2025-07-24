using Application.DTOs.Permissions;
using Application.Interfaces.Common;
using Domain.Entities.AccessControl;

namespace Application.Interfaces.Repositories.AccessControl
{
    public interface IPermissionRepository : IBaseRepository<Permission>
    {
        Task<int> GetRecordCountWhereDynamicAsync(PermissionFilterDto filter);
        Task<IEnumerable<Permission>> SelectSkipAndTakeWhereDynamicAsync(PermissionFilterDto filter);

        Task<bool> ExistsByNameAsync(string permissionName);

        Task<Permission?> GetByNameAsync(string name);
    }
}
