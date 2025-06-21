using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommercialNews.Controllers
{

    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public AuthController(
            IUserService userService,
            IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        /// <summary>
        /// Register a new user and send email verification link.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var domain = $"{Request.Scheme}://{Request.Host}"; // Build verify link base
            var userId = await _userService.RegisterUserAsync(dto, domain);
            return Ok(new { message = "Register successful. Please check your email to verify your account.", userId });
        }

        /// <summary>
        /// Verify user email with token.
        /// </summary>
        [HttpGet("verify")]
        public async Task<IActionResult> Verify([FromQuery] string token)
        {
            await _authService.VerifyEmailTokenAsync(token);
            return Ok(new { message = "Email verified successfully!" });
        }

        /// <summary>
        /// Login and get AccessToken + RefreshToken.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            // result = "access|refresh"
            var parts = result.Split("|");
            return Ok(new
            {
                AccessToken = parts[0],
                RefreshToken = parts[1]
            });
        }

        /// <summary>
        /// Resend verification email if account is not verified.
        /// </summary>
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request)
        {
            var domain = $"{Request.Scheme}://{Request.Host}";
            await _authService.ResendVerificationAsync(request, domain);
            return Ok(new { message = "Verification email has been resent if the account is not yet verified." });
        }


        /// <summary>
        /// Refresh AccessToken and RefreshToken.
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(result);
        }

    }
}
