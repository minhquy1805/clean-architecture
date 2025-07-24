using Application.DTOs.RolePermissions;
using Application.Interfaces.Services.AccessControl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommercialNews.Controllers.Admin
{
    [Route("api/v1/admin/role-permissions")]
    [ApiController]
    [Authorize(Policy = "Permission:Manage")]
    public class AdminRolePermissionController : BaseController
    {
        private readonly IRolePermissionService _rolePermissionService;

        public AdminRolePermissionController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        /// <summary>
        /// ✅ Lấy danh sách quyền theo RoleId
        /// </summary>
        [HttpGet("{roleId}/permissions")]
        public async Task<IActionResult> GetPermissionsByRoleId(int roleId)
        {
            var permissions = await _rolePermissionService.GetPermissionsByRoleIdAsync(roleId);
            return OkResponse(permissions, $"✅ Permissions for RoleId {roleId}.");
        }

        /// <summary>
        /// ✅ Gán quyền cho Role
        /// </summary>
        [HttpPost("assign")]
        public async Task<IActionResult> AddPermissionToRole([FromBody] RolePermissionDto dto)
        {
            await _rolePermissionService.AddPermissionToRoleAsync(dto.RoleId, dto.PermissionId);
            return OkResponse("✅ Permission assigned to role.");
        }

        /// <summary>
        /// ✅ Gỡ quyền khỏi Role
        /// </summary>
        [HttpPost("revoke")]
        public async Task<IActionResult> RemovePermissionFromRole([FromBody] RolePermissionDto dto)
        {
            await _rolePermissionService.RemovePermissionFromRoleAsync(dto.RoleId, dto.PermissionId);
            return OkResponse("✅ Permission revoked from role.");
        }
    }
}
