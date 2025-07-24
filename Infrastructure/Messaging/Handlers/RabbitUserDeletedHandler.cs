using Application.DTOs.Users.Message;
using Application.Interfaces.Messaging.Handlers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Handlers
{
    public class RabbitUserDeletedHandler : IRabbitUserDeletedHandler
    {
        private readonly ILogger<RabbitUserDeletedHandler> _logger;

        public RabbitUserDeletedHandler(ILogger<RabbitUserDeletedHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(UserDeletedEvent message)
        {
            _logger.LogInformation("📥 Received UserDeletedEvent for UserId={UserId}, Email={Email}", message.UserId, message.Email);
            return Task.CompletedTask;
        }
    }
}
