using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserVerificationService _userVerificationService;
        private readonly IEmailService _emailService;
        private readonly ILoginHistoryRepository _loginHistoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenRepository refreshTokenRepository,
            IUserVerificationService userVerificationService,
            IEmailService emailService,
            ILoginHistoryRepository loginHistoryRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
            _userVerificationService = userVerificationService;
            _emailService = emailService;
            _loginHistoryRepository = loginHistoryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetIp() =>
            _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "";

        private string GetAgent() =>
            _httpContextAccessor?.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? "";

        public async Task<string> LoginAsync(LoginRequest request)
        {
            User? user = null;
            try
            {
                user = await _userRepository.GetByEmailAsync(request.Email);

                if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
                    throw new Exception("Invalid email or password.");

                if (user.Flag != "T")
                    throw new Exception("Account is not verified yet.");

                if (!user.IsActive)
                    throw new Exception("Account is deactivated. Please contact admin.");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var accessToken = _jwtTokenGenerator.GenerateToken(claims);

                var refreshToken = new RefreshToken
                {
                    UserId = user.UserId,
                    Token = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    Flag = "T"
                };

                await _refreshTokenRepository.InsertAsync(refreshToken);

                user.LastLoginAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                // ✅ Log SUCCESS
                await _loginHistoryRepository.InsertAsync(new LoginHistory
                {
                    UserId = user.UserId,
                    IsSuccess = true,
                    IpAddress = GetIp(),
                    UserAgent = GetAgent(),
                    Message = "Login success"
                });

                return accessToken + "|" + refreshToken.Token;
            }
            catch (Exception ex)
            {
                // ✅ Log FAIL
                await _loginHistoryRepository.InsertAsync(new LoginHistory
                {
                    UserId = user?.UserId ?? 0,
                    IsSuccess = false,
                    IpAddress = GetIp(),
                    UserAgent = GetAgent(),
                    Message = ex.Message
                });

                throw;
            }
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var existing = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
                if (existing == null || existing.ExpiryDate < DateTime.UtcNow || existing.RevokedAt != null)
                    throw new Exception("Invalid refresh token.");

                var user = await _userRepository.GetByIdAsync(existing.UserId);
                if (user == null)
                    throw new Exception("User not found.");

                if (!user.IsActive)
                    throw new Exception("Account is deactivated. Please contact admin.");

                // Revoke old
                existing.RevokedAt = DateTime.UtcNow;
                var newRefreshToken = Guid.NewGuid().ToString();
                existing.ReplacedByToken = newRefreshToken;
                await _refreshTokenRepository.UpdateAsync(existing);

                // Create new RefreshToken
                var newToken = new RefreshToken
                {
                    UserId = user.UserId,
                    Token = newRefreshToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    Flag = "T"
                };
                await _refreshTokenRepository.InsertAsync(newToken);

                // New AccessToken
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var newAccessToken = _jwtTokenGenerator.GenerateToken(claims);

                // ✅ Log SUCCESS
                await _loginHistoryRepository.InsertAsync(new LoginHistory
                {
                    UserId = user.UserId,
                    IsSuccess = true,
                    IpAddress = GetIp(),
                    UserAgent = GetAgent(),
                    Message = $"RefreshToken success. OldToken={refreshToken}, NewToken={newRefreshToken}"
                });

                return new AuthResultDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                // ✅ Log FAIL
                await _loginHistoryRepository.InsertAsync(new LoginHistory
                {
                    UserId = 0,
                    IsSuccess = false,
                    IpAddress = GetIp(),
                    UserAgent = GetAgent(),
                    Message = $"RefreshToken failed: {ex.Message}"
                });

                throw;
            }
        }

        public async Task ResendVerificationAsync(ResendVerificationRequest request, string domain)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("Email not found!");

            if (user.Flag == "T")
                throw new Exception("Account is already verified.");

            if (!user.IsActive)
                throw new Exception("Account is deactivated. Please contact admin.");

            var token = await _userVerificationService.CreateVerificationTokenAsync(user.UserId);
            var verifyLink = $"{domain}/api/v1/auth/verify?token={token}";
            await _emailService.SendVerificationEmailAsync(user.Email, verifyLink);
        }

        public async Task VerifyEmailTokenAsync(string token)
        {
            await _userVerificationService.VerifyTokenAsync(token);
        }
    }
}
