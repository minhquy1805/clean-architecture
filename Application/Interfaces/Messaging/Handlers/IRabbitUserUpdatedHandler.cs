using Application.DTOs.Users.Message;

namespace Application.Interfaces.Messaging.Handlers
{
    public interface IRabbitUserUpdatedHandler
    {
        Task HandleAsync(UserUpdatedEvent message);
    }
}
