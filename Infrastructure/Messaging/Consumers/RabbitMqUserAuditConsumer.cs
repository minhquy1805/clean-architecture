using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Application.Interfaces.Messaging.Handlers;
using Application.Interfaces.Messaging.Consumers;

namespace Infrastructure.Messaging.Consumers
{
    public class RabbitMqUserAuditConsumer : IRabbitMqConsumer
    {
        private readonly IRabbitMqUserAuditMessageHandler _handler;
        private readonly ILogger<RabbitMqUserAuditConsumer> _logger;
        private const string QueueName = "user-audit-queue";

        public RabbitMqUserAuditConsumer(IRabbitMqUserAuditMessageHandler handler, ILogger<RabbitMqUserAuditConsumer> logger)
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
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                await _handler.HandleAsync(message);
                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume(queue: QueueName, autoAck: false, consumer);
            await Task.Delay(-1, stoppingToken);
        }
    }
}
