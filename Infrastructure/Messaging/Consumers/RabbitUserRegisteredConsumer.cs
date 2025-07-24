using Application.Interfaces.Messaging.Consumers;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces.Messaging.Handlers;
using System.Text;
using System.Text.Json;
using Application.DTOs.Users.Message;

namespace Infrastructure.Messaging.Consumers
{
    public class RabbitUserRegisteredConsumer : IRabbitMqConsumer
    {
        private readonly IServiceProvider _provider;
        private const string QueueName = "user-registered-queue";

        public RabbitUserRegisteredConsumer(IServiceProvider provider)
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
                var handler = scope.ServiceProvider.GetRequiredService<IRabbitUserRegisteredHandler>();

                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<UserRegisteredEvent>(json);
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
