using Application.DTOs.AuditLogs;

namespace Application.Interfaces.Messaging.Producers
{
    public interface IRabbitMqUserAuditMessageProducer
    {
        Task SendAsync(UserAuditMessageDto message);
    }
}
