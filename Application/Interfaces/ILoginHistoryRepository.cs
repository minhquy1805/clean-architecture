using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILoginHistoryRepository
    {
        Task InsertAsync(LoginHistory log);
        Task<IEnumerable<LoginHistory>> GetByUserIdAsync(int userId);
        Task<IEnumerable<LoginHistory>> GetPagingAsync(int? userId, int start, int numberOfRows);
    }
}
