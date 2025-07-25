﻿using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using Application.DTOs.Users.Requests;
using Microsoft.AspNetCore.Authorization;

namespace CommercialNews.Controllers.User
{
    [Authorize]
    [Route("api/v1/user")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

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

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateOwnProfileRequest dto)
        {
            if (CurrentUserId is not int userId)
                return Unauthorized(BadRequestResponse("Unauthorized: Missing or invalid user ID"));

            await _userService.UpdateOwnProfileAsync(userId, dto);
            return OkResponse<string>(null!, "Profile updated successfully!");
        }

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

