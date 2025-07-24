using Application.DTOs.Permissions;
using Application.Interfaces.Repositories.AccessControl;
using Application.Interfaces.Services.AccessControl;
using Application.Mappings;
using Application.Services.Abstract;
using Domain.Entities.AccessControl;

namespace Application.Services
{
    public class PermissionService : BasePagingFilterService<PermissionDto, Permission, PermissionFilterDto>, IPermissionService
    {
        private readonly IPermissionRepository permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
            : base(permissionRepository)
        {
            this.permissionRepository = permissionRepository;
        }

        protected override int GetDtoId(PermissionDto dto) => PermissionMapper.GetDtoId(dto);

        protected override PermissionDto MapToDto(Permission entity) => PermissionMapper.MapToDto(entity);

        protected override Permission MapToEntity(PermissionDto dto) => PermissionMapper.MapToEntity(dto);

        protected override string[] GetAllowedSortFields() =>
            new[] { "PermissionId", "Name", "Module", "Action", "CreatedAt" };

        public override Task<int> GetRecordCountWhereDynamicAsync(PermissionFilterDto filter)
        {
            return permissionRepository.GetRecordCountWhereDynamicAsync(filter);
        }

        public override async Task<IEnumerable<PermissionDto>> SelectSkipAndTakeWhereDynamicAsync(PermissionFilterDto filter)
        {
            var permissions = await permissionRepository.SelectSkipAndTakeWhereDynamicAsync(filter);
            return permissions.Select(MapToDto);
        }

        public Task<bool> ExistsByNameAsync(string name)
        {
            return permissionRepository.ExistsByNameAsync(name);
        }

        public async Task<PermissionDto?> GetByNameAsync(string name) // ✅ Mới
        {
            var permission = await permissionRepository.GetByNameAsync(name);
            return permission == null ? null : PermissionMapper.MapToDto(permission);
        }
    }
}
