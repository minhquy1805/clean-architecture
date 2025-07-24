using Application.Interfaces.Redis.Caching;
using Application.Interfaces.Redis.Tracking;

namespace Infrastructure.Redis.Tracking
{
    public class LoginHistoryZSetTrackerService : ILoginHistoryZSetTrackerService
    {
        private readonly IRedisZSetService _redis;
        private const string RedisKeyPrefix = "login:history";

        public LoginHistoryZSetTrackerService(IRedisZSetService redis)
        {
            _redis = redis;
        }

        private string GetKey(DateTime? date = null)
        {
            return date.HasValue
                ? $"{RedisKeyPrefix}:{date:yyyy-MM-dd}"
                : RedisKeyPrefix;
        }

        public Task IncrementActionAsync(string status, bool useDateSuffix = false)
            => _redis.IncrementActionAsync(GetKey(useDateSuffix ? DateTime.UtcNow : null), status);

        public Task<IEnumerable<(string Status, double Count)>> GetTopActionsAsync(int top = 10, bool useDateSuffix = false)
            => _redis.GetTopAsync(GetKey(useDateSuffix ? DateTime.UtcNow : null), top);

        public Task<double> GetActionCountAsync(string status, bool useDateSuffix = false)
            => _redis.GetScoreAsync(GetKey(useDateSuffix ? DateTime.UtcNow : null), status);

        public Task<bool> RemoveActionAsync(string status, bool useDateSuffix = false)
            => _redis.RemoveMemberAsync(GetKey(useDateSuffix ? DateTime.UtcNow : null), status);

        public Task<bool> ClearAllAsync(bool useDateSuffix = false)
            => _redis.DeleteKeyAsync(GetKey(useDateSuffix ? DateTime.UtcNow : null));

        public async Task<bool> ExistsAsync(string status, bool useDateSuffix = false)
            => (await GetActionCountAsync(status, useDateSuffix)) > 0;

        public async Task<int> CountActionsAsync(bool useDateSuffix = false)
        {
            var key = GetKey(useDateSuffix ? DateTime.UtcNow : null);
            var top = await _redis.GetTopAsync(key, int.MaxValue);
            return top.Count();
        }

        public Task TrackLoginAsync(string status)
            => IncrementActionAsync(status, useDateSuffix: false);

        public Task TrackLoginByDateAsync(string status)
            => IncrementActionAsync(status, useDateSuffix: true);
    }
}
