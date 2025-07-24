namespace Application.Interfaces.Messaging.Consumers
{
    public interface IRabbitMqConsumer
    {
        Task ConsumeAsync(CancellationToken stoppingToken);
    }
}
