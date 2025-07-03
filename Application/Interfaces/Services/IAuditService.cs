

using Application.DTOs.AuditLogs;
using Application.Interfaces.Abstract;


namespace Application.Interfaces.Services
{
    public interface IAuditService : IBasePagingFilterService<UserAuditDto, AuditLogFilterDto>
    {
        Task<IEnumerable<UserAuditDto>> GetByUserIdAsync(int userId);
    }
}
