namespace Application.Interfaces.Messaging.Handlers
{
    public interface IRabbitMqLoginHistoryMessageHandler
    {
        Task HandleAsync(string rawMessage);
    }
}
