using Application.DTOs.Email;
using Application.DTOs.Users.Message;
using Application.Interfaces.Messaging.Producers;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Messaging.Producers
{
    public class RabbitMqUserMessageProducer : IRabbitMqUserMessageProducer
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqUserMessageProducer(IConfiguration config)
        {
            _factory = new ConnectionFactory
            {
                HostName = config["RabbitMq:HostName"] ?? "localhost"
            };
        }

        public Task PublishUserRegisteredAsync(UserRegisteredEvent message)
           => SendAsync("user-registered-queue", message);

        public Task PublishUserUpdatedAsync(UserUpdatedEvent message)
            => SendAsync("user-updated-queue", message);

        public Task PublishUserDeletedAsync(UserDeletedEvent message)
            => SendAsync("user-deleted-queue", message);

        public Task PublishPasswordChangedAsync(UserPasswordChangedEvent message)
            => SendAsync("user-password-changed-queue", message);

        public Task PublishEmailVerifiedAsync(EmailVerifiedEvent evt)
        => SendAsync("email-verified-queue", evt);

        // 🧱 Core logic chia sẻ
        private Task SendAsync(string queueName, object message)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: props, body: body);

            return Task.CompletedTask;
        }

    }
}
