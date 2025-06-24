using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CommercialNews.Responses;

namespace CommercialNews.Controllers.Admin
{
    [Route("api/v1/admin/user")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : BaseController
    {
        private readonly IUserService _userService;

        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// ✅ Get User by Id (Admin view)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFoundResponse("User not found.");
            return OkResponse(user, "Fetched successfully!");
        }

        /// <summary>
        /// ✅ Get all users
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return OkResponse(users, "Fetched all users.");
        }

        /// <summary>
        /// ✅ Get users with paging & filter
        /// </summary>
        [HttpGet("paging")]
        public async Task<ActionResult> GetPaging(
            [FromQuery] string? whereCondition,
            [FromQuery] int start = 0,
            [FromQuery] int rows = 10,
            [FromQuery] string sortBy = "CreatedAt DESC")
        {
            var users = await _userService.SelectSkipAndTakeWhereDynamicAsync(whereCondition ?? "", start, rows, sortBy);
            var total = await _userService.GetRecordCountWhereDynamicAsync(whereCondition ?? "");

            var result = new { Data = users, Total = total };
            return OkResponse(result, "Fetched with paging.");
        }

        /// <summary>
        /// ✅ Get users for dropdown
        /// </summary>
        [HttpGet("dropdown")]
        public async Task<ActionResult> GetDropDownList()
        {
            var dropdown = await _userService.GetDropDownListDataAsync();
            return OkResponse(dropdown, "Fetched dropdown list.");
        }

        /// <summary>
        /// ✅ Update user (Admin)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UserDto dto)
        {
            if (id != dto.UserId)
                return BadRequestResponse("ID mismatch.");

            await _userService.UpdateUserAsync(dto);
            return OkResponse<string>(null!, "User updated successfully!");
        }

        /// <summary>
        /// ✅ Soft Delete user: IsActive = false & revoke token
        /// </summary>
        [HttpPut("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _userService.SoftDeleteUserAsync(id);
            return OkResponse<string>(null!, "User has been soft deleted (IsActive = 0)!");
        }

        /// <summary>
        /// ✅ Restore user: IsActive = true
        /// </summary>
        [HttpPut("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            await _userService.RestoreUserAsync(id);
            return OkResponse<string>(null!, "User has been restored (IsActive = 1)!");
        }

        /// <summary>
        /// ✅ Delete user (Admin)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return OkResponse<string>(null!, "User deleted successfully!");
        }
    }
}
