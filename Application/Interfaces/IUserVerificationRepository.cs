using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserVerificationRepository
    {
        Task<int> InsertAsync(UserVerification verification);
        Task<UserVerification?> GetByTokenAsync(string token);
        Task MarkAsUsedAsync(int userVerificationId);
        Task DeleteExpiredAsync();
    }
}
