using Application.DTOs.Users.Message;
using Application.Interfaces.Messaging.Handlers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Handlers
{
    public class RabbitUserUpdatedHandler : IRabbitUserUpdatedHandler
    {
        private readonly ILogger<RabbitUserUpdatedHandler> _logger;

        public RabbitUserUpdatedHandler(ILogger<RabbitUserUpdatedHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(UserUpdatedEvent message) 
        {
            _logger.LogInformation("📥 Received UserUpdatedEvent for UserId={UserId}, Email={Email}", message.UserId, message.Email);
            return Task.CompletedTask;
        }
    }
}
