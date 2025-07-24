using Application.DTOs.UserRoles;
using Application.Interfaces.Services.AccessControl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommercialNews.Controllers.Admin
{
    [Route("api/v1/admin/user-roles")]
    [ApiController]
    [Authorize(Policy = "Permission:Manage")]
    public class AdminUserRoleController : BaseController
    {
        private readonly IUserRoleService _userRoleService;

        public AdminUserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        /// <summary>
        /// ✅ Lấy danh sách Role theo UserId
        /// </summary>
        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetRolesByUserId(int userId)
        {
            var roles = await _userRoleService.GetRolesByUserIdAsync(userId);
            return OkResponse(roles, $"✅ Roles for UserId {userId}.");
        }

        /// <summary>
        /// ✅ Gán role cho user
        /// </summary>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] UserRoleDto dto)
        {
            await _userRoleService.AddRoleToUserAsync(dto.UserId, dto.RoleId);
            return OkResponse("✅ Role assigned to user.");
        }

        /// <summary>
        /// ✅ Gỡ role khỏi user
        /// </summary>
        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeRoleFromUser([FromBody] UserRoleDto dto)
        {
            await _userRoleService.RemoveRoleFromUserAsync(dto.UserId, dto.RoleId);
            return OkResponse("✅ Role revoked from user.");
        }

       
    }
}
