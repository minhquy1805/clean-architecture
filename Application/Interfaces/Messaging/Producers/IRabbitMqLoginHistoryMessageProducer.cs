using Application.DTOs.LoginHistories;

namespace Application.Interfaces.Messaging.Producers
{
    public interface IRabbitMqLoginHistoryMessageProducer
    {
        Task SendAsync(LoginHistoryMessageDto message);
    }
}
