using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces.Messaging.Handlers;
using System.Text;
using System.Text.Json;
using Application.DTOs.Users.Message;
using Application.Interfaces.Messaging.Consumers;

namespace Infrastructure.Messaging.Consumers
{
    public class RabbitUserUpdatedConsumer : IRabbitMqConsumer
    {
        private readonly IServiceProvider _provider;
        private const string QueueName = "user-updated-queue";

        public RabbitUserUpdatedConsumer(IServiceProvider provider)
        {
            _provider = provider;
        }

        public Task ConsumeAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, ea) =>
            {
                using var scope = _provider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IRabbitUserUpdatedHandler>();

                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<UserUpdatedEvent>(json);
                if (message != null)
                {
                    await handler.HandleAsync(message);
                }
            };

            channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}
