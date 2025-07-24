using Application.DTOs.AuditLogs;
using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IUserAuditRepository : IMongoBaseRepository<UserAudit>
    {
        /// <summary>
        /// Lấy tất cả log theo UserId
        /// </summary>
        Task<IEnumerable<UserAudit>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Truy vấn động + phân trang
        /// </summary>
        Task<IEnumerable<UserAudit>> SelectSkipAndTakeWhereDynamicAsync(AuditLogFilterDto filter);

        /// <summary>
        /// Đếm tổng số bản ghi thỏa điều kiện
        /// </summary>
        Task<int> GetRecordCountWhereDynamicAsync(AuditLogFilterDto filter);

        /// <summary>
        /// Ghi log audit nhanh chóng
        /// </summary>
        Task LogAuditAsync(int userId, string action, object? oldValue, object? newValue, string? ip = null);
    }
}
