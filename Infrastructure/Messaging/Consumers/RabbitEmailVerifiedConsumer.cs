using Application.Interfaces.Messaging.Consumers;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Application.DTOs.Email;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces.Messaging.Handlers;

namespace Infrastructure.Messaging.Consumers
{
    public class RabbitEmailVerifiedConsumer : IRabbitMqConsumer
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<RabbitEmailVerifiedConsumer> _logger;
        private const string QueueName = "email-verified-queue";

        public RabbitEmailVerifiedConsumer(IServiceProvider provider, ILogger<RabbitEmailVerifiedConsumer> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task ConsumeAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<EmailVerifiedEvent>(json);

                    if (message != null)
                    {
                        using var scope = _provider.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<IRabbitEmailVerifiedHandler>();
                        await handler.HandleAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error handling EmailVerifiedEvent");
                }
            };

            channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

            // Keep the task alive
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
