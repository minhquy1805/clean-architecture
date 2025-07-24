namespace Application.Interfaces.Slack
{
    public interface ISlackService
    {
        Task SendAlertAsync(string title, string message);
    }
}
