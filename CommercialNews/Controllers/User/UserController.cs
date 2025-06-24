using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CommercialNews.Responses;

namespace CommercialNews.Controllers.User
{
    [Route("api/v1/user")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// ✅ Get profile of the logged-in user.
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult> GetProfile()
        {
            int userId = CurrentUserId;
            var user = await _userService.GetByIdAsync(userId);
            return OkResponse(user, "Profile fetched successfully!");
        }

        /// <summary>
        /// ✅ Update own profile
        /// </summary>
        [HttpPut("me")]
        public async Task<ActionResult> UpdateMe([FromBody] UserDto dto)
        {
            int userId = CurrentUserId;
            dto.UserId = userId;
            await _userService.UpdateOwnProfileAsync(userId, dto);
            return OkResponse<string>(null!, "Profile updated successfully!");
        }

        /// <summary>
        /// ✅ Change password
        /// </summary>
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            int userId = CurrentUserId;
            await _userService.ChangePasswordAsync(userId, request);
            return OkResponse<string>(null!, "Password changed successfully!");
        }
    }
}
