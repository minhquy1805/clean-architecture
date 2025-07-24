using Application.Interfaces.Repositories.AccessControl;
using Application.Interfaces.Services.Seeding;
using Domain.Constants;

namespace Infrastructure.Seeding
{
    public class RolePermissionSeederService : IRolePermissionSeederService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public RolePermissionSeederService(
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IRolePermissionRepository rolePermissionRepository)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task SeedPermissionsToAdminAsync()
        {
            await SeedPermissionsToRoleAsync("Admin", PermissionConstants.All.ToList());
        }

        public async Task SeedPermissionsToUserAsync()
        {
            var permissions = PermissionConstants
                .GetPermissionsForModule("UserSelf") // ✅ chỉ dùng quyền cá nhân
                .Concat(PermissionConstants.GetPermissionsForModule("Dashboard")) // có thể cho user xem dashboard
                .ToList();

            await SeedPermissionsToRoleAsync("User", permissions);
        }

        public async Task SeedPermissionsToModeratorAsync()
        {
            var permissions = PermissionConstants
                .GetPermissionsForModule("User")
                .Concat(PermissionConstants.GetPermissionsForModule("AuditLog"))
                .ToList();

            await SeedPermissionsToRoleAsync("Moderator", permissions);
        }

        public async Task SeedPermissionsToRoleAsync(string roleName, List<string> permissionNames)
        {
            if (string.IsNullOrWhiteSpace(roleName) || permissionNames == null || permissionNames.Count == 0)
                return;

            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role == null) return;

            var existingIds = (await _rolePermissionRepository
                .GetPermissionIdsByRoleIdAsync(role.RoleId))
                .ToHashSet(); // ✅ HashSet để Contains nhanh hơn

            foreach (var name in permissionNames.Distinct())
            {
                var permission = await _permissionRepository.GetByNameAsync(name);
                if (permission != null && !existingIds.Contains(permission.PermissionId))
                {
                    await _rolePermissionRepository.InsertAsync(role.RoleId, permission.PermissionId);
                }
            }
        }
    }
}
