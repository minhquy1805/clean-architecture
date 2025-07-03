using Application.DTOs.LoginHistories;
using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ILoginHistoryRepository : IBaseRepository<LoginHistory>
    {
        Task<IEnumerable<LoginHistory>> GetByUserIdAsync(int userId);

        Task<IEnumerable<LoginHistory>> SelectSkipAndTakeWhereDynamicAsync(LoginHistoryFilterDto filter);

        Task<int> GetRecordCountWhereDynamicAsync(LoginHistoryFilterDto filter);
    }
}
