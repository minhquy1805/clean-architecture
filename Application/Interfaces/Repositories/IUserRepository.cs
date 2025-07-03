using Application.DTOs.Users.Filters;
using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        // Các hàm mở rộng không có trong BaseRepository
        Task<User?> GetByEmailAsync(string email);
        Task UpdateIsActiveAsync(int userId, bool isActive);
        Task<IEnumerable<User>> GetAllWhereDynamicAsync(UserFilterDto filter);
        Task<IEnumerable<User>> SelectSkipAndTakeAsync(int start, int rows, string sortBy);
        Task<IEnumerable<User>> SelectSkipAndTakeWhereDynamicAsync(UserFilterDto filter);
        Task<int> GetRecordCountAsync();
        Task<int> GetRecordCountWhereDynamicAsync(UserFilterDto filter);
        Task<IEnumerable<User>> GetDropDownListDataAsync();
    }
}
