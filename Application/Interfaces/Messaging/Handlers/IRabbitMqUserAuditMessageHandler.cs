namespace Application.Interfaces.Messaging.Handlers
{
    public interface IRabbitMqUserAuditMessageHandler
    {
        Task HandleAsync(string rawMessage);
    }
}
