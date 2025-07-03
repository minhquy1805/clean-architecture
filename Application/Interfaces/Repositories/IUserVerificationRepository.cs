using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IUserVerificationRepository : IBaseRepository<UserVerification>
    {
        Task<UserVerification?> GetByTokenAsync(string token);
        Task MarkAsUsedAsync(int userVerificationId);
        Task DeleteExpiredAsync();
    }
}
