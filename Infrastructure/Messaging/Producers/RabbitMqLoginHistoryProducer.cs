using Amazon.Runtime.Internal.Util;
using Application.DTOs.LoginHistories;
using Application.Interfaces.Messaging.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Messaging.Producers
{
    public class RabbitMqLoginHistoryProducer : IRabbitMqLoginHistoryMessageProducer
    {
        private const string QueueName = "login-history-queue";
        private readonly ConnectionFactory _factory;
        private readonly ILogger<RabbitMqLoginHistoryProducer> _logger;

        public RabbitMqLoginHistoryProducer(IConfiguration config, ILogger<RabbitMqLoginHistoryProducer> logger)
        {
            _logger = logger;
            _factory = new ConnectionFactory
            {
                HostName = config["RabbitMq:HostName"] ?? "localhost"
            };
        }

        public Task SendAsync(LoginHistoryMessageDto message)
        {
            try
            {
                using var connection = _factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var props = channel.CreateBasicProperties();
                props.Persistent = true;

                channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: props, body: body);

                _logger.LogInformation("📤 [LoginHistoryProducer] Sent message to queue '{Queue}': {Message}", QueueName, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [LoginHistoryProducer] Failed to send message to queue '{Queue}'", QueueName);
            }

            return Task.CompletedTask;
        }
    }
}
