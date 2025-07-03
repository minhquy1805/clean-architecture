
using Application.DTOs.AuditLogs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.Services.Abstract;
using Domain.Entities;

namespace Application.Services
{
    public class AuditService
        : BasePagingFilterService<UserAuditDto, UserAudit, AuditLogFilterDto>, IAuditService
    {
        private readonly IUserAuditRepository _repo;

        public AuditService(IUserAuditRepository repo) : base(repo)
        {
            _repo = repo;
        }

        protected override UserAuditDto MapToDto(UserAudit entity) => AuditMapper.ToDto(entity);

        protected override UserAudit MapToEntity(UserAuditDto dto) => AuditMapper.ToEntity(dto);

        protected override int GetDtoId(UserAuditDto dto) => dto.AuditId;

        protected override string[] GetAllowedSortFields() =>
            new[] { "AuditId", "UserId", "Action", "CreatedAt", "Flag", "Field1", "Field2" };

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
    }
}
