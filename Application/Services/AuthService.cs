using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenRepository refreshTokenRepository,
            IUserVerificationService userVerificationService,
            IEmailService emailService) // Thêm DI
         {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
            _userVerificationService = userVerificationService;
            _emailService = emailService;
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new Exception("Invalid email or password.");

            if(user.Flag != "T")
                throw new Exception("Account is not verified yet.");

            // Standard Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var accessToken = _jwtTokenGenerator.GenerateToken(claims);

            // Create RefreshToken
            var refreshToken = new RefreshToken
            {
                UserId = user.UserId,
                Token = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                Flag = "T"
            };

            await _refreshTokenRepository.InsertAsync(refreshToken);

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return accessToken + "|" + refreshToken.Token;
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken) 
        {
            var existing = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (existing == null || existing.ExpiryDate < DateTime.UtcNow || existing.RevokedAt != null)
                throw new Exception("Invalid refresh token.");

            // Get user
            var user = await _userRepository.GetByIdAsync(existing.UserId);
            if(user == null)
                throw new Exception("User not found.");

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

            return new AuthResultDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task ResendVerificationAsync(ResendVerificationRequest request, string domain)
        {
            // Tìm user theo email
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("Email not found!");

            if (user.Flag == "T")
                throw new Exception("Account is already verified.");

            // Tạo token mới
            var token = await _userVerificationService.CreateVerificationTokenAsync(user.UserId);

            // Build link mới
            var verifyLink = $"{domain}/api/v1/auth/verify?token={token}";

            // Gửi lại email
            await _emailService.SendVerificationEmailAsync(user.Email, verifyLink);
        }

        public async Task VerifyEmailTokenAsync(string token)
        {
            await _userVerificationService.VerifyTokenAsync(token);
        }
    }
}
