using Application.DTOs.Users.Message;

namespace Application.Interfaces.Messaging.Handlers
{
    public interface IRabbitUserPasswordChangedHandler
    {
        Task HandleAsync(UserPasswordChangedEvent message);
    }
}
