using Application.DTOs.Permissions;
using Application.Interfaces.Services.AccessControl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommercialNews.Controllers.Admin
{
    [Route("api/v1/admin/permissions")]
    [ApiController]
    [Authorize(Policy = "Permission:Manage")]
    public class AdminPermissionController : BaseController
    {
        private readonly IPermissionService _permissionService;

        public AdminPermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// ✅ Get all permissions (no paging)
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permissions = await _permissionService.GetAllAsync();
            return OkResponse(permissions, "✅ All permissions fetched successfully.");
        }

        /// <summary>
        /// ✅ Get permissions with paging + filter
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPermissionsPaged([FromQuery] PermissionFilterDto filter)
        {
            var data = await _permissionService.SelectSkipAndTakeWhereDynamicAsync(filter);
            var count = await _permissionService.GetRecordCountWhereDynamicAsync(filter);

            return OkResponse(new
            {
                Total = count,
                Data = data
            }, "✅ Paged permissions fetched successfully.");
        }

        /// <summary>
        /// ✅ Get permission by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var permission = await _permissionService.GetByIdAsync(id);
            if (permission == null)
                return NotFoundResponse("❌ Permission not found.");

            return OkResponse(permission, "✅ Permission fetched successfully.");
        }
    }
}
