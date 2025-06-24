using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetAllWhereDynamicAsync(string whereCondition);
        Task<IEnumerable<User>> SelectSkipAndTakeAsync(int start, int rows, string sortBy);
        Task<IEnumerable<User>> SelectSkipAndTakeWhereDynamicAsync(string whereCondition, int start, int rows, string sortBy);
        Task<int> GetRecordCountAsync();
        Task<int> GetRecordCountWhereDynamicAsync(string whereCondition);
        Task<IEnumerable<User>> GetDropDownListDataAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<int> InsertAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task UpdateIsActiveAsync(int userId, bool isActive);
    }
}
