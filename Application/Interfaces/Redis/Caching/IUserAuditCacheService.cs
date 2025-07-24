namespace Application.Interfaces.Redis.Caching
{
    public interface IUserAuditCacheService
    {
        Task SetLastAuditAsync(int userId, string action, DateTime timestamp);
        Task<DateTime?> GetLastAuditTimeAsync(int userId);
        Task<string?> GetLastAuditActionAsync(int userId);
    }
}
