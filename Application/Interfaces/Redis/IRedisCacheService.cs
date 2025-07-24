namespace Application.Interfaces.Redis
{
    public interface IRedisCacheService
    {
        // For list-based caching (e.g., audit logs) danh sách (List)
        Task AddToListAsync<T>(string key, T item, int maxLength, TimeSpan expiry);
        Task<List<T>> GetListAsync<T>(string key);
        Task SetListAsync<T>(string key, List<T> data, TimeSpan expiry);
        Task<long> CountListAsync(string key);

        // For object-level caching (dạng key-value)
        Task SetAsync<T>(string key, T value, TimeSpan expiry);
        Task<T?> GetAsync<T>(string key);

        // Management Kiểm tra / xóa key
        Task<bool> KeyExistsAsync(string key);
        Task RemoveAsync(string key);
    }
}
