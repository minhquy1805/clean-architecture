using Application.DTOs.AuditLogs;
using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IUserAuditRepository : IBaseRepository<UserAudit>
    {
        Task<IEnumerable<UserAudit>> GetByUserIdAsync(int userId);

        // New methods using AuditLogFilterDto
        Task<IEnumerable<UserAudit>> SelectSkipAndTakeWhereDynamicAsync(AuditLogFilterDto filter);
        Task<int> GetRecordCountWhereDynamicAsync(AuditLogFilterDto filter);
    }
}
