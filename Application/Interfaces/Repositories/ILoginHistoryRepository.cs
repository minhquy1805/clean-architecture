using Application.DTOs.LoginHistories;
using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ILoginHistoryRepository : IMongoBaseRepository<LoginHistory>
    {
        Task<IEnumerable<LoginHistory>> GetByUserIdAsync(int userId);
        Task<LoginHistory?> GetLastLoginAsync(int userId);

        // ✅ Thêm lại filter + paging
        Task<IEnumerable<LoginHistory>> SelectSkipAndTakeWhereDynamicAsync(LoginHistoryFilterDto filter);
        Task<int> GetRecordCountWhereDynamicAsync(LoginHistoryFilterDto filter);
    }
}
