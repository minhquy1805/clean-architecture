using Application.Common.Errors;
using Application.DTOs.Roles;
using Application.Interfaces.Common;
using Application.Interfaces.Repositories.AccessControl;
using Application.Interfaces.Services;
using Application.Interfaces.Services.AccessControl;
using Application.Mappings;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserContext _currentUserContext;

        public UserRoleService(
            IUserRoleRepository userRoleRepository,
            IAuditService auditService,
            IHttpContextAccessor httpContextAccessor,
            ICurrentUserContext currentUserContext)
        {
            _userRoleRepository = userRoleRepository;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
            _currentUserContext = currentUserContext;
        }

        public async Task AddRoleToUserAsync(int userId, int roleId)
        {
            if (userId <= 0 || roleId <= 0)
                throw UserRoleErrors.InvalidUserOrRole();

            var existingRoleIds = await _userRoleRepository.GetRoleIdsByUserIdAsync(userId);
            if (existingRoleIds.Contains(roleId))
                throw UserRoleErrors.AlreadyAssigned(userId, roleId);

            await _userRoleRepository.InsertAsync(userId, roleId);

            await _auditService.LogAuditAsync(new UserAudit
            {
                UserId = userId,
                Action = $"AssignRole",
                OldValue = null,
                NewValue = $"RoleId={roleId}",
                IpAddress = _currentUserContext.GetIpAddress(),
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task RemoveRoleFromUserAsync(int userId, int roleId)
        {
            await _userRoleRepository.DeleteAsync(userId, roleId);

            await _auditService.LogAuditAsync(new UserAudit
            {
                UserId = userId,
                Action = $"RemoveRole",
                OldValue = $"RoleId={roleId}",
                NewValue = null,
                IpAddress = _currentUserContext.GetIpAddress(),
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<IEnumerable<RoleDto>> GetRolesByUserIdAsync(int userId)
        {
            var roles = await _userRoleRepository.GetRolesByUserIdAsync(userId);
            return roles.Select(RoleMapper.MapToDto);
        }

        public Task<IEnumerable<int>> GetRoleIdsByUserIdAsync(int userId)
        {
            return _userRoleRepository.GetRoleIdsByUserIdAsync(userId);
        }

        public async Task<Dictionary<int, List<RoleDto>>> GetRolesByUserIdsAsync(IEnumerable<int> userIds)
        {
            var raw = await _userRoleRepository.GetRolesByUserIdsAsync(userIds);

            return raw
                .GroupBy(r => r.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => RoleMapper.MapToDto(x.Role)).ToList()
                );
        }

        
    }
}
