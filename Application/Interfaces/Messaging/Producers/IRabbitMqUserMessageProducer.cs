using Application.DTOs.Email;
using Application.DTOs.Users.Message;

namespace Application.Interfaces.Messaging.Producers
{
    public interface IRabbitMqUserMessageProducer
    {
        Task PublishUserRegisteredAsync(UserRegisteredEvent evt);
        Task PublishUserUpdatedAsync(UserUpdatedEvent evt);
        Task PublishUserDeletedAsync(UserDeletedEvent evt);
        Task PublishPasswordChangedAsync(UserPasswordChangedEvent evt);
        Task PublishEmailVerifiedAsync(EmailVerifiedEvent evt);
    }
}
