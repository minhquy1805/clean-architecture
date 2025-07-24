

using Application.DTOs.AuditLogs;
using Application.Interfaces.Abstract;
using Domain.Entities;


namespace Application.Interfaces.Services
{
    public interface IAuditService : IBasePagingFilterServiceMongo<UserAuditDto, AuditLogFilterDto>
    {
        Task<IEnumerable<UserAuditDto>> GetByUserIdAsync(int userId);

        // Log từ hệ thống (Controller gọi trực tiếp)
        Task LogAuditAsync(int userId, string action, object? oldValue, object? newValue);

        // Log từ RabbitMQ Consumer gọi vào
        Task LogAuditAsync(UserAudit audit);

        //Producer
        Task LogAsync(UserAuditMessageDto dto);
    }
}
