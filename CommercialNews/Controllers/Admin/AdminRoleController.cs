using Application.DTOs.Roles;
using Application.Interfaces.Services.AccessControl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommercialNews.Controllers.Admin
{
    [Route("api/v1/admin/roles")]
    [ApiController]
    [Authorize(Policy = "Permission:Manage")]
    public class AdminRoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public AdminRoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// ✅ Get all roles (no paging)
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllAsync();
            return OkResponse(roles, "✅ All roles fetched successfully.");
        }

        /// <summary>
        /// ✅ Get roles with paging + filter
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRolesPaged([FromQuery] RoleFilterDto filter)
        {
            var result = await _roleService.SelectSkipAndTakeWhereDynamicAsync(filter);
            var count = await _roleService.GetRecordCountWhereDynamicAsync(filter);

            return OkResponse(new
            {
                Total = count,
                Data = result
            }, "✅ Paged roles fetched successfully.");
        }

        /// <summary>
        /// ✅ Get role by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
                return NotFoundResponse("❌ Role not found.");

            return OkResponse(role, "✅ Role fetched successfully.");
        }
    }
}
