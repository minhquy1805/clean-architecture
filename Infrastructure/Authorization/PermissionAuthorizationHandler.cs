using Application.Interfaces.Repositories.AccessControl;
using Application.Interfaces.Services.AccessControl;
using Domain.Entities.AccessControl;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Infrastructure.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUserRoleService _userRoleService;
        private readonly IRolePermissionService _rolePermissionService;

        public PermissionAuthorizationHandler(
            IUserRoleService userRoleService,
            IRolePermissionService rolePermissionService)
        {
            _userRoleService = userRoleService;
            _rolePermissionService = rolePermissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return;

            var roleIds = await _userRoleService.GetRoleIdsByUserIdAsync(userId);
            if (!roleIds.Any()) return;

            var permissions = await _rolePermissionService.GetPermissionsByRoleIdsAsync(roleIds);

            if (permissions.Any(p => p.Name == requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}
