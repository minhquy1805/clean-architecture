using Application.DTOs;
using Application.DTOs.LoginHistories;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.Services.Abstract;
using Domain.Entities;

namespace Application.Services
{
    public class LoginHistoryService
        : BasePagingFilterService<LoginHistoryDto, LoginHistory, LoginHistoryFilterDto>, ILoginHistoryService
    {
        private readonly ILoginHistoryRepository _repo;

        public LoginHistoryService(ILoginHistoryRepository repo) : base(repo)
        {
            _repo = repo;
        }

        protected override LoginHistoryDto MapToDto(LoginHistory entity) => LoginHistoryMapper.ToDto(entity);

        protected override LoginHistory MapToEntity(LoginHistoryDto dto) => LoginHistoryMapper.ToEntity(dto);

        protected override int GetDtoId(LoginHistoryDto dto) => dto.LoginId;

        protected override string[] GetAllowedSortFields() =>
            new[] { "LoginId", "UserId", "CreatedAt", "IsSuccess" };

        public async Task<IEnumerable<LoginHistoryDto>> GetByUserIdAsync(int userId)
        {
            var logs = await _repo.GetByUserIdAsync(userId);
            return logs.Select(LoginHistoryMapper.ToDto);
        }

        public override async Task<IEnumerable<LoginHistoryDto>> SelectSkipAndTakeWhereDynamicAsync(LoginHistoryFilterDto filter)
        {
            var entities = await _repo.SelectSkipAndTakeWhereDynamicAsync(filter);
            return entities.Select(LoginHistoryMapper.ToDto);
        }

        public override Task<int> GetRecordCountWhereDynamicAsync(LoginHistoryFilterDto filter)
        {
            return _repo.GetRecordCountWhereDynamicAsync(filter);
        }
    }
}
