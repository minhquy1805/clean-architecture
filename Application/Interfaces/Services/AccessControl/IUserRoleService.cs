using Application.DTOs.Roles;

namespace Application.Interfaces.Services.AccessControl
{
    public interface IUserRoleService
    {
        Task AddRoleToUserAsync(int userId, int roleId);
        Task RemoveRoleFromUserAsync(int userId, int roleId);
        Task<IEnumerable<RoleDto>> GetRolesByUserIdAsync(int userId);
        Task<IEnumerable<int>> GetRoleIdsByUserIdAsync(int userId);

        Task<Dictionary<int, List<RoleDto>>> GetRolesByUserIdsAsync(IEnumerable<int> userIds);
    }
}
