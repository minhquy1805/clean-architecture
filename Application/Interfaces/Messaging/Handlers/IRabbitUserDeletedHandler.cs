using Application.DTOs.Users.Message;

namespace Application.Interfaces.Messaging.Handlers
{
    public interface IRabbitUserDeletedHandler
    {
        Task HandleAsync(UserDeletedEvent message);
    }
}
