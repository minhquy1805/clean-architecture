using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserAuditRepository
    {
        Task InsertAsync(UserAudit audit);

        Task<IEnumerable<UserAudit>> GetByUserIdAsync(int userId);

        Task<IEnumerable<UserAudit>> GetPagingAsync(int? userId, int start, int numberOfRows);
    }
}
