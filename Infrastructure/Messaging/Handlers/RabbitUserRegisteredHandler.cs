using Application.DTOs.Users.Message;
using Application.Interfaces.Messaging.Handlers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Handlers
{
    public class RabbitUserRegisteredHandler : IRabbitUserRegisteredHandler
    {
        private readonly ILogger<RabbitUserRegisteredHandler> _logger;

        public RabbitUserRegisteredHandler(ILogger<RabbitUserRegisteredHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(UserRegisteredEvent message)
        {
            _logger.LogInformation("📥 Received UserRegisteredEvent for UserId={UserId}, Email={Email}", message.UserId, message.Email);
            return Task.CompletedTask;
        }
    }
}
