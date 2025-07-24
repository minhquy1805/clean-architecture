using Domain.Entities.AccessControl;

namespace Application.Interfaces.Repositories.AccessControl
{
    public interface IUserRoleRepository
    {
        Task InsertAsync(int userId, int roleId);
        Task DeleteAsync(int userId, int roleId);
        Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId);
        Task<IEnumerable<int>> GetRoleIdsByUserIdAsync(int userId);

        Task<IEnumerable<(int UserId, Role Role)>> GetRolesByUserIdsAsync(IEnumerable<int> userIds);
    }
}


