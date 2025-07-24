namespace Application.Interfaces.Redis.Caching
{
    public interface IRedisZSetService
    {
        Task IncrementActionAsync(string key, string member);
        Task<double> GetScoreAsync(string key, string member);
        Task<IEnumerable<(string Member, double Score)>> GetTopAsync(string key, int top = 10);
        Task<bool> RemoveMemberAsync(string key, string member);
        Task<bool> DeleteKeyAsync(string key);
    }
}
