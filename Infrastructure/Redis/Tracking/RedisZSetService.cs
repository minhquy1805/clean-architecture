using Application.Interfaces.Redis.Caching;
using StackExchange.Redis;

namespace Infrastructure.Redis.Tracking
{
    public class RedisZSetService : IRedisZSetService
    {
        private readonly IDatabase _redis;

        public RedisZSetService(IConnectionMultiplexer multiplexer)
        {
            _redis = multiplexer.GetDatabase();
        }

        public Task IncrementActionAsync(string key, string member)
        {
            return _redis.SortedSetIncrementAsync(key, member, 1);
        }

        public async Task<double> GetScoreAsync(string key, string member)
        {
            var score = await _redis.SortedSetScoreAsync(key, member);
            return score ?? 0;
        }

        public async Task<IEnumerable<(string Member, double Score)>> GetTopAsync(string key, int top = 10)
        {
            var results = await _redis.SortedSetRangeByRankWithScoresAsync(key, -top, -1, Order.Descending);
            return results.Select(x => (x.Element.ToString(), x.Score));
        }

        public Task<bool> RemoveMemberAsync(string key, string member)
        {
            return _redis.SortedSetRemoveAsync(key, member);
        }

        public Task<bool> DeleteKeyAsync(string key)
        {
            return _redis.KeyDeleteAsync(key);
        }
    }
}
