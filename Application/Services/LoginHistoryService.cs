using Application.DTOs.LoginHistories;
using Application.Interfaces.Redis.Caching;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.Services.Abstract;
using Domain.Entities;

namespace Application.Services
{
    public class LoginHistoryService
         : BasePagingFilterServiceMongo<LoginHistoryDto, LoginHistory, LoginHistoryFilterDto>, ILoginHistoryService
    {
        private readonly ILoginHistoryRepository _loginHistoryRepository;
        private readonly ILoginHistoryCacheService _cache;

        public LoginHistoryService(
             ILoginHistoryRepository loginHistoryRepository,
             ILoginHistoryCacheService cache
         )
            : base(loginHistoryRepository)
        {
            _loginHistoryRepository = loginHistoryRepository;
            _cache = cache;
        }

        protected override string GetDtoId(LoginHistoryDto dto)
        {
            return dto.LoginHistoryId;
        }

        protected override string[] GetAllowedSortFields()
        {
            return LoginHistoryFilterDto.AllowedSortFields;
        }

        protected override LoginHistoryDto MapToDto(LoginHistory entity)
        {
            return LoginHistoryMapper.ToDto(entity);
        }

        protected override LoginHistory MapToEntity(LoginHistoryDto dto)
        {
            return LoginHistoryMapper.ToEntity(dto);
        }

        public async Task<IEnumerable<LoginHistoryDto>> GetByUserIdAsync(int userId)
        {
            var entities = await _loginHistoryRepository.GetByUserIdAsync(userId);
            return entities.Select(MapToDto);
        }

        public async Task<LoginHistoryDto?> GetLastLoginAsync(int userId)
        {
            // ✅ Check cache first
            var cached = await _cache.GetLastLoginAsync(userId);
            if (cached != null)
                return cached;

            // ❌ Not found in cache → get from DB
            var entity = await _loginHistoryRepository.GetLastLoginAsync(userId);
            if (entity == null) return null;

            var dto = MapToDto(entity);
            await _cache.SetLastLoginAsync(userId, dto); // ✅ Save to cache

            return dto;
        }

        public override async Task<IEnumerable<LoginHistoryDto>> SelectSkipAndTakeWhereDynamicAsync(LoginHistoryFilterDto filter)
        {
            var entities = await _loginHistoryRepository.SelectSkipAndTakeWhereDynamicAsync(filter);
            return entities.Select(MapToDto);
        }

        public override Task<int> GetRecordCountWhereDynamicAsync(LoginHistoryFilterDto filter)
        {
            return _loginHistoryRepository.GetRecordCountWhereDynamicAsync(filter);
        }
    }
}
