using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using Application.DTOs.Users.Requests;

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
            if (CurrentUserId is not int userId)
                return Unauthorized(BadRequestResponse("Unauthorized: Missing or invalid user ID"));

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return NotFoundResponse("User not found");

            return OkResponse(user, "Profile fetched successfully!");
        }

        /// <summary>
        /// ✅ Update own profile
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateOwnProfileRequest dto)
        {
            if (CurrentUserId is not int userId)
                return Unauthorized(BadRequestResponse("Unauthorized: Missing or invalid user ID"));

            await _userService.UpdateOwnProfileAsync(userId, dto);
            return OkResponse<string>(null!, "Profile updated successfully!");
        }

        /// <summary>
        /// ✅ Change password
        /// </summary>
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (CurrentUserId is not int userId)
                return Unauthorized(BadRequestResponse("Unauthorized: Missing or invalid user ID"));

            await _userService.ChangePasswordAsync(userId, request);
            return OkResponse<string>(null!, "Password changed successfully!");
        }
    }
}
