namespace Application.Interfaces.Redis.Tracking
{
    public interface IAuditZSetTrackerService
    {
        Task IncrementActionAsync(string action, bool useDateSuffix = false);
        Task<IEnumerable<(string Action, double Count)>> GetTopActionsAsync(int top = 10, bool useDateSuffix = false);
        Task<double> GetActionCountAsync(string action, bool useDateSuffix = false);
        Task<bool> RemoveActionAsync(string action, bool useDateSuffix = false);
        Task<bool> ClearAllAsync(bool useDateSuffix = false);
        Task<bool> ExistsAsync(string action, bool useDateSuffix = false);
        Task<int> CountActionsAsync(bool useDateSuffix = false);
        Task TrackActionAsync(string action);
        Task TrackActionByDateAsync(string action);
    }
}
