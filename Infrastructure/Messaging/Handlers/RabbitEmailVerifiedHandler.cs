using Application.DTOs.Email;
using Application.Interfaces.Messaging.Handlers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Handlers
{
    public class RabbitEmailVerifiedHandler : IRabbitEmailVerifiedHandler
    {
        private readonly ILogger<RabbitEmailVerifiedHandler> _logger;

        public RabbitEmailVerifiedHandler(ILogger<RabbitEmailVerifiedHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(EmailVerifiedEvent message)
        {
            _logger.LogInformation("📥 Received EmailVerifiedEvent: UserId={UserId}, Email={Email}", message.UserId, message.Email);
            return Task.CompletedTask;
        }
    }
}
