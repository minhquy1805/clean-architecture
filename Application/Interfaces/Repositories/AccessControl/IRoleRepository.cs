using Application.DTOs.Roles;
using Application.Interfaces.Common;
using Domain.Entities.AccessControl;

namespace Application.Interfaces.Repositories.AccessControl
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<int> GetRecordCountWhereDynamicAsync(RoleFilterDto filter);
        Task<IEnumerable<Role>> SelectSkipAndTakeWhereDynamicAsync(RoleFilterDto filter);
        Task<Role?> GetByNameAsync(string roleName);
        Task<bool> ExistsByNameAsync(string roleName);
    }
}
