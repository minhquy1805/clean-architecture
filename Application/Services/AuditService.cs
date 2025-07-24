using Application.DTOs.AuditLogs;
using Application.Interfaces.Messaging.Producers;
using Application.Interfaces.Redis.Caching;
using Application.Interfaces.Redis.Tracking;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.Services.Abstract;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Application.Services
{
    public class AuditService
        : BasePagingFilterServiceMongo<UserAuditDto, UserAudit, AuditLogFilterDto>, IAuditService
    {
        private readonly IUserAuditRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRabbitMqUserAuditMessageProducer _producer;
        private readonly IAuditZSetTrackerService _tracker;
        private readonly IUserAuditCacheService _cacheService; // ✅ Inject thêm

        public AuditService(
            IUserAuditRepository repo,
            IHttpContextAccessor httpContextAccessor,
            IRabbitMqUserAuditMessageProducer producer,
            IAuditZSetTrackerService tracker,
            IUserAuditCacheService cacheService // ✅ Thêm vào constructor
        ) : base(repo)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
            _producer = producer;
            _tracker = tracker;
            _cacheService = cacheService;
        }

        protected override UserAuditDto MapToDto(UserAudit entity) => AuditMapper.ToDto(entity);

        protected override UserAudit MapToEntity(UserAuditDto dto) => AuditMapper.ToEntity(dto);

        protected override string GetDtoId(UserAuditDto dto) => dto.AuditId;

        protected override string[] GetAllowedSortFields() => AuditLogFilterDto.AllowedSortFields;

        public async Task<IEnumerable<UserAuditDto>> GetByUserIdAsync(int userId)
        {
            var audits = await _repo.GetByUserIdAsync(userId);
            return audits.Select(AuditMapper.ToDto);
        }

        public override async Task<IEnumerable<UserAuditDto>> SelectSkipAndTakeWhereDynamicAsync(AuditLogFilterDto filter)
        {
            var audits = await _repo.SelectSkipAndTakeWhereDynamicAsync(filter);
            return audits.Select(AuditMapper.ToDto);
        }

        public override Task<int> GetRecordCountWhereDynamicAsync(AuditLogFilterDto filter)
        {
            return _repo.GetRecordCountWhereDynamicAsync(filter);
        }

        /// <summary>
        /// ✅ Ghi log trực tiếp (hiếm dùng), vẫn giữ để consumer dùng
        /// </summary>
        public async Task LogAuditAsync(UserAudit audit)
        {
            await _repo.InsertAsync(audit);
            await _tracker.TrackActionAsync(audit.Action);
            await _tracker.TrackActionByDateAsync(audit.Action);

            // ✅ Cập nhật cache
            await _cacheService.SetLastAuditAsync(audit.UserId, audit.Action, audit.CreatedAt);
        }

        /// <summary>
        /// ✅ Gửi message RabbitMQ (gọi từ các service khác như Auth)
        /// </summary>
        public async Task LogAuditAsync(int userId, string action, object? oldValue, object? newValue)
        {
            var message = new UserAuditMessageDto
            {
                UserId = userId,
                Action = action,
                OldValue = oldValue != null ? SerializeForAudit(oldValue) : null,
                NewValue = newValue != null ? SerializeForAudit(newValue) : null,
                IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                CreatedAt = DateTime.UtcNow,
                Flag = "T"
            };

            await _producer.SendAsync(message);

            // ✅ Cập nhật cache ngay sau khi gửi message (tối ưu dashboard hoặc theo dõi)
            await _cacheService.SetLastAuditAsync(userId, action, message.CreatedAt);
        }

        public async Task LogAsync(UserAuditMessageDto dto)
        {
            await _producer.SendAsync(dto);

            // ✅ Bổ sung update cache cho hàm này nếu được gọi từ bên ngoài
            await _cacheService.SetLastAuditAsync(dto.UserId, dto.Action, dto.CreatedAt);
        }

        private new string SerializeForAudit(object obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}
