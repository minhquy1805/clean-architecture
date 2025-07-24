using Application.DTOs.AuditLogs;
using Application.Interfaces.Messaging.Producers;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Messaging.Producers
{
    public class RabbitMqAuditProducer : IRabbitMqUserAuditMessageProducer
    {
        private const string QueueName = "user-audit-queue";
        private readonly ConnectionFactory _factory;

        public RabbitMqAuditProducer(IConfiguration config)
        {
            _factory = new ConnectionFactory
            {
                HostName = config["RabbitMq:HostName"] ?? "localhost"
            };
        }

        public Task SendAsync(UserAuditMessageDto message)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: props, body: body);

            return Task.CompletedTask;
        }
    }
}
