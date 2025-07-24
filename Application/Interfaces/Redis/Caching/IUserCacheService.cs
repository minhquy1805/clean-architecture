using Application.DTOs.Users.Responses;

namespace Application.Interfaces.Redis.Caching
{
    public interface IUserCacheService
    {
        Task<UserDto?> GetByIdAsync(int userId);
        Task<UserDto?> GetByEmailAsync(string email);
        Task SetAsync(UserDto user);
        Task RemoveAsync(int userId, string email);
    }
}
