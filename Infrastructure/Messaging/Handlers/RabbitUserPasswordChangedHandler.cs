using Application.DTOs.Users.Message;
using Application.Interfaces.Messaging.Handlers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Handlers
{
    public class RabbitUserPasswordChangedHandler : IRabbitUserPasswordChangedHandler
    {
        private readonly ILogger<RabbitUserPasswordChangedHandler> _logger;

        public RabbitUserPasswordChangedHandler(ILogger<RabbitUserPasswordChangedHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(UserPasswordChangedEvent message)
        {
            _logger.LogInformation("📥 Received UserPasswordChangedEvent for UserId={UserId}, Email={Email}", message.UserId, message.Email);
            return Task.CompletedTask;
        }
    }
}
