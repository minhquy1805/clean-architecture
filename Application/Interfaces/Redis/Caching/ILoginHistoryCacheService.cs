using Application.DTOs.LoginHistories;

namespace Application.Interfaces.Redis.Caching
{
    public interface ILoginHistoryCacheService
    {
        Task SetLastLoginAsync(int userId, LoginHistoryDto dto);
        Task<LoginHistoryDto?> GetLastLoginAsync(int userId);
    }
}
