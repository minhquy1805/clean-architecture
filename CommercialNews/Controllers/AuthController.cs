using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using Application.DTOs.Auth.Requests;
using Application.DTOs.Users.Requests;

namespace CommercialNews.Controllers
{

    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(
            IUserService userService,
            IAuthService authService,
            IConfiguration config)
        {
            _userService = userService;
            _authService = authService;
            _config = config;
        }

        /// <summary>
        /// Register a new user and send email verification link.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var domain = $"{Request.Scheme}://{Request.Host}";
            var userId = await _userService.RegisterUserAsync(dto, domain);
            return OkResponse(userId, "Register successful. Please check your email to verify your account.");
        }

        /// <summary>
        /// Verify user email with token.
        /// </summary>
        [HttpGet("verify")]
        public async Task<IActionResult> Verify([FromQuery] string token)
        {
            await _authService.VerifyEmailTokenAsync(token);
            return OkResponse<string>(null!, "Email verified successfully!");
        }

        /// <summary>
        /// Login and get AccessToken + RefreshToken.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            var parts = result.Split("|");
            var response = new
            {
                AccessToken = parts[0],
                RefreshToken = parts[1]
            };

            return OkResponse(response, "Login successful.");
        }

        /// <summary>
        /// Resend verification email if account is not verified.
        /// </summary>
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request)
        {
            var domain = $"{Request.Scheme}://{Request.Host}";
            await _authService.ResendVerificationAsync(request, domain);
            return OkResponse<string>(null!, "Verification email has been resent if the account is not yet verified.");
        }

        /// <summary>
        /// Refresh AccessToken and RefreshToken.
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            return OkResponse(result, "Token refreshed successfully.");
        }

        /// <summary>
        /// ✅ Forgot Password: send reset link to email.
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var domain = _config["App:Domain"];
            var verifyLinkBase = $"{domain}/auth";
            await _userService.ForgotPasswordAsync(request.Email, verifyLinkBase);
            return OkResponse<string>(null!, "Please check your email to reset password!");
        }

        /// <summary>
        /// ✅ Reset Password with token.
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _userService.ResetPasswordAsync(request);
            return OkResponse<string>(null!, "Password has been reset successfully.");
        }
    }
}
