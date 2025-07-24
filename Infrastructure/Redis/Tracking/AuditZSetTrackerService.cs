using Application.Interfaces.Redis.Caching;
using Application.Interfaces.Redis.Tracking;

namespace Infrastructure.Redis.Tracking
{
    public class AuditZSetTrackerService : IAuditZSetTrackerService
    {
        private readonly IRedisZSetService _redis;
        private const string RedisKeyPrefix = "audit:actions";

        public AuditZSetTrackerService(IRedisZSetService redis)
        {
            _redis = redis;
        }

        private string GetKey(DateTime? date = null)
        {
            return date.HasValue
                ? $"{RedisKeyPrefix}:{date:yyyy-MM-dd}"
                : RedisKeyPrefix;
        }

        // 🔹 Gốc
        public Task IncrementActionAsync(string action, bool useDateSuffix = false)
          => _redis.IncrementActionAsync(GetKey(useDateSuffix ? DateTime.UtcNow : null), action);

        public Task<IEnumerable<(string Action, double Count)>> GetTopActionsAsync(int top = 10, bool useDateSuffix = false)
            => _redis.GetTopAsync(GetKey(useDateSuffix ? DateTime.UtcNow : null), top);

        public Task<double> GetActionCountAsync(string action, bool useDateSuffix = false)
            => _redis.GetScoreAsync(GetKey(useDateSuffix ? DateTime.UtcNow : null), action);

        public Task<bool> ClearAllAsync(bool useDateSuffix = false)
            => _redis.DeleteKeyAsync(GetKey(useDateSuffix ? DateTime.UtcNow : null));


        // 🔹 Mở rộng

        // ✅ Xóa 1 action cụ thể
        public Task<bool> RemoveActionAsync(string action, bool useDateSuffix = false)
            => _redis.RemoveMemberAsync(GetKey(useDateSuffix ? DateTime.UtcNow : null), action);

        // ✅ Kiểm tra action có tồn tại không
        public async Task<bool> ExistsAsync(string action, bool useDateSuffix = false)
            => (await GetActionCountAsync(action, useDateSuffix)) > 0;

        // ✅ Đếm tổng số action loại khác nhau (cardinality)
        public async Task<int> CountActionsAsync(bool useDateSuffix = false)
        {
            var key = GetKey(useDateSuffix ? DateTime.UtcNow : null);
            var top = await _redis.GetTopAsync(key, int.MaxValue);
            return top.Count();
        }

        public Task TrackActionAsync(string action)
        {
            return IncrementActionAsync(action, useDateSuffix: false);
        }

        public Task TrackActionByDateAsync(string action)
        {
            return IncrementActionAsync(action, useDateSuffix: true);
        }
    }
}
