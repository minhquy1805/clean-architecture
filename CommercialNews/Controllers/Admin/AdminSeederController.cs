using Application.DTOs.Email;
using Application.DTOs.Seeding;
using Application.Interfaces.Services;
using Infrastructure.Seeding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommercialNews.Controllers.Admin
{
    [Route("api/v1/admin/seeder")]
    [ApiController]
    [Authorize(Policy = "Permission:Manage")]
    public class AdminSeederController : BaseController
    {
        private readonly RoleSeederService _roleSeeder;
        private readonly PermissionSeederService _permissionSeeder;
        private readonly RolePermissionSeederService _rolePermissionSeeder;
        private readonly UserRoleSeederService _userRoleSeeder;
        private readonly IAuditService _auditService;
        private readonly DefaultAdminSettings _adminSettings;

        public AdminSeederController(
           RoleSeederService roleSeeder,
           PermissionSeederService permissionSeeder,
           RolePermissionSeederService rolePermissionSeeder,
           UserRoleSeederService userRoleSeeder,
           IAuditService auditService,
           DefaultAdminSettings adminSettings)
        {
            _roleSeeder = roleSeeder;
            _permissionSeeder = permissionSeeder;
            _rolePermissionSeeder = rolePermissionSeeder;
            _userRoleSeeder = userRoleSeeder;
            _auditService = auditService;
            _adminSettings = adminSettings;
        }

        /// <summary>
        /// ✅ Seed tất cả: Role + Permission + Gán quyền cho Admin
        /// </summary>
        [HttpPost("seed-all")]
        public async Task<IActionResult> SeedAllAsync()
        {
            await _roleSeeder.SeedDefaultRolesAsync();
            await _permissionSeeder.SeedDefaultPermissionsAsync();
            await _rolePermissionSeeder.SeedPermissionsToAdminAsync();

            await _auditService.LogAuditAsync(
                CurrentUserId ?? 0,
                "SeedAll",
                null,
                "Seeded Roles, Permissions, Admin Role-Permissions"
            );

            return OkResponse("✅ All roles, permissions, and Admin mappings have been seeded.");
        }

        [HttpPost("user-role")]
        public async Task<IActionResult> SeedUserRoleAsync([FromBody] UserRoleSeedRequestDto dto)
        {
            await _userRoleSeeder.SeedUserRoleAsync(dto.Email, dto.RoleName, _adminSettings.Password); // 👈 dùng password từ config

            await _auditService.LogAuditAsync(
                CurrentUserId ?? 0,
                "SeedUserRole",
                null,
                dto
            );

            return OkResponse($"✅ User '{dto.Email}' đã được gán vào role '{dto.RoleName}'.");
        }



        [HttpPost("roles")]
        public async Task<IActionResult> SeedRoleAsync([FromBody] RoleSeedRequestDto dto)
        {
            await _roleSeeder.SeedRoleIfNotExistsAsync(dto.RoleName);

            await _auditService.LogAuditAsync(
               CurrentUserId ?? 0,
               "SeedRole",
               null,
               dto
           );

            return OkResponse($"✅ Role '{dto.RoleName}' đã được tạo nếu chưa tồn tại.");
        }

        [HttpPost("permissions")]
        public async Task<IActionResult> SeedPermissionsAsync([FromBody] PermissionSeedRequestDto dto)
        {
            await _permissionSeeder.SeedPermissionsAsync(dto.Permissions);

            await _auditService.LogAuditAsync(
               CurrentUserId ?? 0,
               "SeedPermissions",
               null,
               dto
           );

            return OkResponse("✅ Các permission đã được tạo nếu chưa tồn tại.");
        }

        /// <summary>
        /// 🔄 Gán lại Permission cho Role Admin
        /// </summary>
        [HttpPost("admin-role-permissions")]
        public async Task<IActionResult> SeedAdminRolePermissionsAsync([FromBody] PermissionSeedRequestDto dto)
        {
            await _rolePermissionSeeder.SeedPermissionsToRoleAsync(dto.RoleName, dto.Permissions);

            await _auditService.LogAuditAsync(
                CurrentUserId ?? 0,
                "SeedAdminRolePermissions",
                null,
                dto
            );

            return OkResponse($"✅ Permissions assigned to role '{dto.RoleName}'.");
        }

        [HttpPost("moderator-role-permissions")]
        public async Task<IActionResult> SeedModeratorRolePermissionsAsync()
        {
            await _rolePermissionSeeder.SeedPermissionsToModeratorAsync();

            await _auditService.LogAuditAsync(
               CurrentUserId ?? 0,
               "SeedModeratorRolePermissions",
               null,
               "Moderator permissions seeded"
           );

            return OkResponse("✅ Permissions assigned to Moderator role.");
        }
    }
}
