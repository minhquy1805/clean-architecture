using Application.DTOs.LoginHistories;
using Application.Interfaces.Redis.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Redis.Caching
{
    public class LoginHistoryCacheService : ILoginHistoryCacheService
    {
        private readonly IDistributedCache _cache;
        private const string Prefix = "user:{0}:last_login";

        public LoginHistoryCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetLastLoginAsync(int userId, LoginHistoryDto dto)
        {
            var key = string.Format(Prefix, userId);
            var json = JsonSerializer.Serialize(dto);
            await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
            });
        }

        public async Task<LoginHistoryDto?> GetLastLoginAsync(int userId)
        {
            var key = string.Format(Prefix, userId);
            var json = await _cache.GetStringAsync(key);
            return json is null ? null : JsonSerializer.Deserialize<LoginHistoryDto>(json);
        }
    }
}
