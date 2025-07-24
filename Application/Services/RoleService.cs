using Application.DTOs.Roles;
using Application.Interfaces.Repositories.AccessControl;
using Application.Interfaces.Services.AccessControl;
using Application.Mappings;
using Application.Services.Abstract;
using Domain.Entities.AccessControl;

namespace Application.Services
{
    public class RoleService : BasePagingFilterService<RoleDto, Role, RoleFilterDto>, IRoleService
    {
        private readonly IRoleRepository roleRepository;

        public RoleService(IRoleRepository roleRepository)
            : base(roleRepository)
        {
            this.roleRepository = roleRepository;
        }

        protected override int GetDtoId(RoleDto dto) => RoleMapper.GetDtoId(dto);

        protected override RoleDto MapToDto(Role entity) => RoleMapper.MapToDto(entity);

        protected override Role MapToEntity(RoleDto dto) => RoleMapper.MapToEntity(dto);

        protected override string[] GetAllowedSortFields() =>
           new[] { "RoleId", "RoleName", "Flag" };

        public override Task<int> GetRecordCountWhereDynamicAsync(RoleFilterDto filter)
        {
            return roleRepository.GetRecordCountWhereDynamicAsync(filter);
        }

        public override async Task<IEnumerable<RoleDto>> SelectSkipAndTakeWhereDynamicAsync(RoleFilterDto filter)
        {
            var roles = await roleRepository.SelectSkipAndTakeWhereDynamicAsync(filter);
            return roles.Select(MapToDto);
        }

        public async Task<RoleDto?> GetByNameAsync(string roleName)
        {
            var role = await roleRepository.GetByNameAsync(roleName);
            return role == null ? null : RoleMapper.MapToDto(role);
        }

        public Task<bool> ExistsByNameAsync(string roleName)
        {
            return roleRepository.ExistsByNameAsync(roleName);
        }
    }
}
