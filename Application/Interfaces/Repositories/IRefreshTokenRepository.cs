using Application.Interfaces.Common;
using Domain.Entities;


namespace Application.Interfaces
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task DeleteByUserIdAsync(int userId);
    }
}
