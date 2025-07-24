using Application.Interfaces.Redis.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Redis.Caching
{
    public class UserAuditCacheService : IUserAuditCacheService
    {
        private readonly IDistributedCache _cache;

        public UserAuditCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        private string GetTimeKey(int userId) => $"user:{userId}:last_audit_time";
        private string GetActionKey(int userId) => $"user:{userId}:last_audit_action";

        public async Task SetLastAuditAsync(int userId, string action, DateTime timestamp)
        {
            var timeKey = GetTimeKey(userId);
            var actionKey = GetActionKey(userId);

            var timeValue = timestamp.ToString("O"); // ISO 8601 format

            await _cache.SetStringAsync(timeKey, timeValue);
            await _cache.SetStringAsync(actionKey, action);
        }

        public async Task<DateTime?> GetLastAuditTimeAsync(int userId)
        {
            var timeKey = GetTimeKey(userId);
            var value = await _cache.GetStringAsync(timeKey);
            return DateTime.TryParse(value, out var result) ? result : null;
        }

        public async Task<string?> GetLastAuditActionAsync(int userId)
        {
            var actionKey = GetActionKey(userId);
            return await _cache.GetStringAsync(actionKey);
        }
    }
}
