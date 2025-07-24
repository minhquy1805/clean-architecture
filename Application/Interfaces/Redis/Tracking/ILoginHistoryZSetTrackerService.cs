namespace Application.Interfaces.Redis.Tracking
{
    public interface ILoginHistoryZSetTrackerService
    {
        Task IncrementActionAsync(string status, bool useDateSuffix = false);
        Task<IEnumerable<(string Status, double Count)>> GetTopActionsAsync(int top = 10, bool useDateSuffix = false);
        Task<double> GetActionCountAsync(string status, bool useDateSuffix = false);
        Task<bool> RemoveActionAsync(string status, bool useDateSuffix = false);
        Task<bool> ClearAllAsync(bool useDateSuffix = false);
        Task<bool> ExistsAsync(string status, bool useDateSuffix = false);
        Task<int> CountActionsAsync(bool useDateSuffix = false);
        Task TrackLoginAsync(string status);
        Task TrackLoginByDateAsync(string status);
    }
}
