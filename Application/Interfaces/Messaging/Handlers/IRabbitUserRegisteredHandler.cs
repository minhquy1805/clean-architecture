using Application.DTOs.Users.Message;

namespace Application.Interfaces.Messaging.Handlers
{
    public interface IRabbitUserRegisteredHandler
    {
        Task HandleAsync(UserRegisteredEvent message);
    }
}
