using Application.Interfaces.Messaging.Consumers;
using Application.Interfaces.Messaging.Handlers;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace Infrastructure.Messaging.Consumers
{
    public class RabbitMqLoginHistoryConsumer : IRabbitMqConsumer
    {
        private readonly IRabbitMqLoginHistoryMessageHandler _handler;
        private readonly ILogger<RabbitMqLoginHistoryConsumer> _logger;
        private const string QueueName = "login-history-queue";

        public RabbitMqLoginHistoryConsumer(IRabbitMqLoginHistoryMessageHandler handler, ILogger<RabbitMqLoginHistoryConsumer> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public async Task ConsumeAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("📨 Received login history message: {Message}", message);

                try
                {
                    await _handler.HandleAsync(message);
                    channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("✅ Processed login history message successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error handling login history message.");
                    // Optional: channel.BasicNack(ea.DeliveryTag, false, true); // for retry
                }
            };

            channel.BasicConsume(queue: QueueName, autoAck: false, consumer);

            _logger.LogInformation("🚀 LoginHistoryConsumer started. Listening on queue: {Queue}", QueueName);

            await Task.Delay(-1, stoppingToken);
        }
    }
}
