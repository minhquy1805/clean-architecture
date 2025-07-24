using Application.Common.Errors;
using Application.DTOs.Permissions;
using Application.Interfaces.Common;
using Application.Interfaces.Repositories.AccessControl;
using Application.Interfaces.Services;
using Application.Interfaces.Services.AccessControl;
using Application.Mappings;
using Domain.Entities;
using Microsoft.AspNetCore.Http;


namespace Application.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserContext _currentUserContext;

        public RolePermissionService(
            IRolePermissionRepository rolePermissionRepository,
            IAuditService auditService,
            IHttpContextAccessor httpContextAccessor,
            ICurrentUserContext currentUserContext)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
            _currentUserContext = currentUserContext;
        }

        public async Task AddPermissionToRoleAsync(int roleId, int permissionId)
        {
            if (roleId <= 0 || permissionId <= 0)
                throw RolePermissionErrors.InvalidRoleOrPermission();

            var existing = await _rolePermissionRepository.GetPermissionIdsByRoleIdAsync(roleId);
            if (existing.Contains(permissionId))
                throw RolePermissionErrors.AlreadyAssigned(roleId, permissionId);

            await _rolePermissionRepository.InsertAsync(roleId, permissionId);

            await _auditService.LogAuditAsync(new UserAudit
            {
                UserId = _currentUserContext.GetUserId(),
                Action = $"AddPermissionToRole",
                OldValue = null,
                NewValue = $"{{ RoleId: {roleId}, PermissionId: {permissionId} }}",
                IpAddress = _currentUserContext.GetIpAddress(),
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var existing = await _rolePermissionRepository.GetPermissionIdsByRoleIdAsync(roleId);
            if (!existing.Contains(permissionId))
                throw RolePermissionErrors.NotAssigned(roleId, permissionId);

            await _rolePermissionRepository.DeleteAsync(roleId, permissionId);

            await _auditService.LogAuditAsync(new UserAudit
            {
                UserId = _currentUserContext.GetUserId(),
                Action = $"RemovePermissionFromRole",
                OldValue = $"{{ RoleId: {roleId}, PermissionId: {permissionId} }}",
                NewValue = null,
                IpAddress = _currentUserContext.GetIpAddress(),
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleIdAsync(int roleId)
        {
            var permissions = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(roleId);
            return permissions.Select(PermissionMapper.MapToDto);
        }

        public Task<IEnumerable<int>> GetPermissionIdsByRoleIdAsync(int roleId)
        {
            return _rolePermissionRepository.GetPermissionIdsByRoleIdAsync(roleId);
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleIdsAsync(IEnumerable<int> roleIds)
        {
            var permissions = await _rolePermissionRepository.GetPermissionsByRoleIdsAsync(roleIds);
            return permissions.Select(PermissionMapper.MapToDto);
        }
    }
}
