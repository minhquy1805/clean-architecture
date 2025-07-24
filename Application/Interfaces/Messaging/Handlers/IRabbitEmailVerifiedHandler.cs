using Application.DTOs.Email;

namespace Application.Interfaces.Messaging.Handlers
{
    public interface IRabbitEmailVerifiedHandler
    {
        Task HandleAsync(EmailVerifiedEvent message);
    }
}
