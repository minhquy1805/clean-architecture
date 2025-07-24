using Application.DTOs.Users.Responses;
using Application.Interfaces.Redis;
using Application.Interfaces.Redis.Caching;

namespace Infrastructure.Redis.Caching
{
    public class UserCacheService : IUserCacheService
    {
        private readonly IRedisCacheService _redis;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public UserCacheService(IRedisCacheService redis)
        {
            _redis = redis;
        }

        // 📌 Lấy user từ cache theo UserId
        public Task<UserDto?> GetByIdAsync(int userId)
        {
            return _redis.GetAsync<UserDto>(GetUserIdKey(userId));
        }

        // 📌 Lấy user từ cache theo Email
        public Task<UserDto?> GetByEmailAsync(string email)
        {
            return _redis.GetAsync<UserDto>(GetUserEmailKey(email));
        }

        // ✅ Set user cache theo cả UserId và Email
        public async Task SetAsync(UserDto user)
        {
            var idKey = GetUserIdKey(user.UserId);
            var emailKey = GetUserEmailKey(user.Email);

            await _redis.SetAsync(idKey, user, CacheDuration);
            await _redis.SetAsync(emailKey, user, CacheDuration);
        }

        // ❌ Xoá cache user theo cả UserId và Email
        public async Task RemoveAsync(int userId, string email)
        {
            await _redis.RemoveAsync(GetUserIdKey(userId));
            await _redis.RemoveAsync(GetUserEmailKey(email));
        }

        // 🔐 Key Helpers
        private static string GetUserIdKey(int userId) => $"user:id:{userId}";
        private static string GetUserEmailKey(string email) => $"user:email:{email.ToLower()}";
    }
}

