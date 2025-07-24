using Application.Interfaces.Redis;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace Infrastructure.Redis.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConfiguration configuration)
        {
            var connection = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]!);
            _db = connection.GetDatabase();
        }

        // -----------------------------
        // 🔹 List-based caching (e.g. logs)
        // -----------------------------

        public async Task AddToListAsync<T>(string key, T item, int maxLength, TimeSpan expiry)
        {
            var json = JsonConvert.SerializeObject(item);
            await _db.ListLeftPushAsync(key, json);
            await _db.ListTrimAsync(key, 0, maxLength - 1);
            await _db.KeyExpireAsync(key, expiry);
        }

        public async Task<List<T>> GetListAsync<T>(string key)
        {
            var values = await _db.ListRangeAsync(key);
            return values
                .Select(v => JsonConvert.DeserializeObject<T>(v!))
                .Where(x => x != null)
                .ToList()!;
        }

        public async Task SetListAsync<T>(string key, List<T> data, TimeSpan expiry)
        {
            await _db.KeyDeleteAsync(key);

            if (data == null || data.Count == 0)
                return;

            var values = data
                .Select(d => (RedisValue)JsonConvert.SerializeObject(d))
                .ToArray();

            await _db.ListRightPushAsync(key, values);
            await _db.KeyExpireAsync(key, expiry);
        }

        public async Task<long> CountListAsync(string key)
        {
            return await _db.ListLengthAsync(key);
        }

        // -----------------------------
        // 🔹 Object-based caching (key-value)
        // -----------------------------

        public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
        {
            var json = JsonConvert.SerializeObject(value);
            await _db.StringSetAsync(key, json, expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return default;

            return JsonConvert.DeserializeObject<T>(value!);
        }

        // -----------------------------
        // 🔹 Key management
        // -----------------------------

        public async Task<bool> KeyExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}
