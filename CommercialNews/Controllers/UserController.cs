using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommercialNews.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// ✅ Get User By Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Get profile of the logged-in user.
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _userService.GetByIdAsync(userId);
            return Ok(user);
        }

        /// <summary>
        /// Update profile
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UserDto dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            dto.UserId = userId;
            await _userService.UpdateOwnProfileAsync(userId, dto);
            return Ok(new { message = "Profile updated successfully!" });
        }

        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _userService.ChangePasswordAsync(userId, request);
            return Ok(new { message = "Password changed successfully!" });
        }

        /// <summary>
        /// ✅ Get All Users
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// ✅ Get Users with Paging & Filter
        /// </summary>

        [HttpGet("paging")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetPaging(
           [FromQuery] string? whereCondition,
           [FromQuery] int start = 0,
           [FromQuery] int rows = 10,
           [FromQuery] string sortBy = "CreatedAt DESC")
        {
            var users = await _userService.SelectSkipAndTakeWhereDynamicAsync(whereCondition ?? "", start, rows, sortBy);
            var total = await _userService.GetRecordCountWhereDynamicAsync(whereCondition ?? "");
            return Ok(new { data = users, total });
        }

        /// <summary>
        /// ✅ Get Users for Dropdown List
        /// </summary>
        [HttpGet("dropdown")]
        public async Task<ActionResult<IEnumerable<UserDropDownDto>>> GetDropDownList()
        {
            var dropdown = await _userService.GetDropDownListDataAsync();
            return Ok(dropdown);
        }

        /// <summary>
        /// ✅ Update User
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto dto)
        {
            if (id != dto.UserId) return BadRequest("ID mismatch.");
            await _userService.UpdateUserAsync(dto);
            return Ok(new { message = "Updated successfully!" });
        }

        /// <summary>
        /// ✅ Delete User
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new { message = "Deleted successfully!" });
        }
    }
}
