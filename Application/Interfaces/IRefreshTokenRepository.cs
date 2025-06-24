using Domain.Entities;


namespace Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task InsertAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task UpdateAsync(RefreshToken token);

        Task DeleteByUserIdAsync(int userId);
    }
}
