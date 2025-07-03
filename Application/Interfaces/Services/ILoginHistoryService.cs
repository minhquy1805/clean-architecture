using Application.DTOs.LoginHistories;
using Application.Interfaces.Abstract;

namespace Application.Interfaces.Services
{
    public interface ILoginHistoryService : IBasePagingFilterService<LoginHistoryDto, LoginHistoryFilterDto>
    {
        Task<IEnumerable<LoginHistoryDto>> GetByUserIdAsync(int userId);
    }
}
