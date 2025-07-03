// Refactored AuthService: Clean structure, consistent error handling, AppException, logging

using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Application.Common.Errors;
using Application.DTOs.Auth.Requests;
using Application.DTOs.Auth.Jwt;

namespace Application.Services
{
    public record AuthServiceDependencies(
        IUserRepository UserRepository,
        IPasswordHasher PasswordHasher,
        IJwtTokenGenerator JwtTokenGenerator,
        IRefreshTokenRepository RefreshTokenRepository,
        IUserVerificationService UserVerificationService,
        IEmailService EmailService,
        ILoginHistoryRepository LoginHistoryRepository,
        IHttpContextAccessor HttpContextAccessor,
        IOptions<JwtSettings> JwtOptions
    );

    public class AuthService : IAuthService
    {
        private readonly AuthServiceDependencies _dep;
        private readonly JwtSettings _jwtSettings;

        public AuthService(AuthServiceDependencies dep)
        {
            _dep = dep;
            _jwtSettings = dep.JwtOptions.Value;
        }

        private string GetIp() =>
            _dep.HttpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "";

        private string GetAgent() =>
            _dep.HttpContextAccessor?.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? "";

        public async Task<string> LoginAsync(LoginRequest request)
        {
            User? user = null;
            try
            {
                user = await _dep.UserRepository.GetByEmailAsync(request.Email)
                    ?? throw AuthErrors.InvalidCredentials();

                if (!_dep.PasswordHasher.VerifyPassword(user.PasswordHash, request.Password))
                    throw AuthErrors.InvalidCredentials();


                if (user.Flag != "T")
                    throw AuthErrors.NotVerified();

                if (!user.IsActive)
                    throw UserErrors.InactiveAccount();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var accessToken = _dep.JwtTokenGenerator.GenerateToken(claims, _jwtSettings.TokenExpiryMinutes);
                var refreshTokenValue = Guid.NewGuid().ToString();

                await _dep.RefreshTokenRepository.InsertAsync(new RefreshToken
                {
                    UserId = user.UserId,
                    Token = refreshTokenValue,
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                    Flag = "T",
                    IPAddress = GetIp(),
                    UserAgent = GetAgent()
                });

                user.LastLoginAt = DateTime.UtcNow;
                await _dep.UserRepository.UpdateAsync(user);

                await LogLogin(user.UserId, true, "Login success");

                return accessToken + "|" + refreshTokenValue;
            }
            catch (Exception ex)
            {
                await LogLogin(user?.UserId ?? 0, false, ex.Message);
                throw;
            }
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var existing = await _dep.RefreshTokenRepository.GetByTokenAsync(refreshToken)
                    ?? throw AuthErrors.InvalidRefreshToken();

                if (existing.ExpiryDate < DateTime.UtcNow || existing.RevokedAt != null)
                    throw AuthErrors.ExpiredOrRevokedToken();

                var user = await _dep.UserRepository.GetByIdAsync(existing.UserId)
                    ?? throw UserErrors.NotFound();

                if (!user.IsActive)
                    throw UserErrors.InactiveAccount();

                existing.RevokedAt = DateTime.UtcNow;
                var newRefreshToken = Guid.NewGuid().ToString();
                existing.ReplacedByToken = newRefreshToken;
                await _dep.RefreshTokenRepository.UpdateAsync(existing);

                await _dep.RefreshTokenRepository.InsertAsync(new RefreshToken
                {
                    UserId = user.UserId,
                    Token = newRefreshToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    Flag = "T"
                });

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var newAccessToken = _dep.JwtTokenGenerator.GenerateToken(claims, _jwtSettings.TokenExpiryMinutes);

                await LogLogin(user.UserId, true, $"Refresh token success. Old={refreshToken}, New={newRefreshToken}");

                return new AuthResultDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                await LogLogin(0, false, $"Refresh token failed: {ex.Message}");
                throw;
            }
        }

        public async Task ResendVerificationAsync(ResendVerificationRequest request, string domain)
        {
            var user = await _dep.UserRepository.GetByEmailAsync(request.Email)
                ?? throw UserErrors.NotFound("For resend verification");

            if (user.Flag == "T")
                throw UserErrors.AlreadyVerified();

            if (!user.IsActive)
                throw UserErrors.InactiveAccount();

            var token = await _dep.UserVerificationService.CreateVerificationTokenAsync(user.UserId);
            var verifyLink = $"{domain}/api/v1/auth/verify?token={token}";
            await _dep.EmailService.SendVerificationEmailAsync(user.Email, verifyLink);
        }

        public async Task VerifyEmailTokenAsync(string token)
        {
            await _dep.UserVerificationService.VerifyTokenAsync(token);
        }

        private async Task LogLogin(int userId, bool isSuccess, string message)
        {
            await _dep.LoginHistoryRepository.InsertAsync(new LoginHistory
            {
                UserId = userId,
                IsSuccess = isSuccess,
                IpAddress = GetIp(),
                UserAgent = GetAgent(),
                Message = message
            });
        }
    }
}
