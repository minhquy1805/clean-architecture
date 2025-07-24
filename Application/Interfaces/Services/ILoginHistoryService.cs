using Application.DTOs.LoginHistories;
using Application.Interfaces.Abstract;

namespace Application.Interfaces.Services
{
    public interface ILoginHistoryService
        : IBasePagingFilterServiceMongo<LoginHistoryDto, LoginHistoryFilterDto>
    {
        Task<IEnumerable<LoginHistoryDto>> GetByUserIdAsync(int userId);

        Task<LoginHistoryDto?> GetLastLoginAsync(int userId);
    }
}
