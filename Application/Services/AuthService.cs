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
using Application.DTOs.AuditLogs;
using System.Text.Json;
using Application.Interfaces.Messaging.Producers;
using Application.DTOs.LoginHistories;
using Application.Helpers;
using Application.Interfaces.Redis.Caching;
using Application.DTOs.Users.Responses;
using Application.Mappings;
using Application.Common.Helpers;
using Application.DTOs.Users.Message;
using Application.DTOs.Email;

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
        IOptions<JwtSettings> JwtOptions,
        IAuditService AuditService,
        IRabbitMqLoginHistoryMessageProducer LoginHistoryProducer,
        IUserCacheService UserCache,
        IRabbitMqUserMessageProducer UserProducer
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

        // 👇 Thêm helper mapper ở đây
        private static UserDto MapToDto(User user) => UserMapper.ToDto(user);
        private static User MapToEntity(UserDto dto) => UserMapper.ToEntity(dto);

        public async Task<string> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw AuthErrors.InvalidCredentials();

            User? user = null;

            try
            {
                // ✅ B1: Lấy từ cache (nếu có)
                var cached = await _dep.UserCache.GetByEmailAsync(request.Email);
                if (cached == null)
                {
                    var userFromDb = await _dep.UserRepository.GetByEmailAsync(request.Email)
                        ?? throw AuthErrors.InvalidCredentials();

                    cached = MapToDto(userFromDb);

                    // 👇 Cache lại để lần sau dùng
                    await _dep.UserCache.SetAsync(cached);
                }

                // ✅ B2: Kiểm tra PasswordHash trước khi verify
                if (string.IsNullOrWhiteSpace(cached.PasswordHash))
                {
                    // 👉 Nếu cache lỗi, xoá cache và truy DB lại
                    await _dep.UserCache.RemoveAsync(cached.UserId, cached.Email);
                    var userFromDb = await _dep.UserRepository.GetByEmailAsync(request.Email)
                        ?? throw AuthErrors.InvalidCredentials();

                    Console.WriteLine($"[Debug] Cached.PasswordHash = {cached.PasswordHash}");


                    cached = MapToDto(userFromDb);
                    await _dep.UserCache.SetAsync(cached);
                }

                // ✅ B3: Map sang entity để xử lý logic
                user = MapToEntity(cached);

                // ✅ B4: Xác thực mật khẩu
                if (!_dep.PasswordHasher.VerifyPassword(user.PasswordHash, request.Password))
                    throw AuthErrors.InvalidCredentials();

                if (user.Flag != "T")
                    throw AuthErrors.NotVerified();

                if (!user.IsActive)
                    throw UserErrors.InactiveAccount();

                // ✅ B5: Tạo access token & refresh token
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
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

                // ✅ B6: Cập nhật LastLoginAt
                user.LastLoginAt = DateTime.UtcNow;
                await _dep.UserRepository.UpdateAsync(user);

                // ✅ B7: Ghi log thành công
                Console.WriteLine("🔥 Logging audit: LoginSuccess");
                await LogLogin(user.UserId, true, "Login success", user.Email);

                // ✅ B8: Cập nhật cache lại với dữ liệu mới
                await _dep.UserCache.RemoveAsync(user.UserId, user.Email);
                await _dep.UserCache.SetAsync(MapToDto(user));

                return accessToken + "|" + refreshTokenValue;
            }
            catch (Exception ex)
            {
                await LogLogin(user?.UserId ?? 0, false, ex.Message, request.Email);
                throw;
            }
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // ✅ B1: Lấy refresh token từ DB
                var existing = await _dep.RefreshTokenRepository.GetByTokenAsync(refreshToken)
                    ?? throw AuthErrors.InvalidRefreshToken();

                if (existing.ExpiryDate < DateTime.UtcNow || existing.RevokedAt != null)
                    throw AuthErrors.ExpiredOrRevokedToken();

                // ✅ B2: Lấy user từ cache → fallback sang DB
                var cached = await _dep.UserCache.GetByIdAsync(existing.UserId);
                var user = cached != null
                    ? MapToEntity(cached)
                    : await _dep.UserRepository.GetByIdAsync(existing.UserId) ?? throw UserErrors.NotFound();

                if (!user.IsActive)
                    throw UserErrors.InactiveAccount();

                // ✅ B3: Cập nhật token
                existing.RevokedAt = DateTime.UtcNow;
                var newRefreshToken = Guid.NewGuid().ToString();
                existing.ReplacedByToken = newRefreshToken;
                await _dep.RefreshTokenRepository.UpdateAsync(existing);

                await _dep.RefreshTokenRepository.InsertAsync(new RefreshToken
                {
                    UserId = user.UserId,
                    Token = newRefreshToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                    Flag = "T"
                });

                // ✅ B4: Tạo access token
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

                var newAccessToken = _dep.JwtTokenGenerator.GenerateToken(claims, _jwtSettings.TokenExpiryMinutes);

                // ✅ B5: Ghi audit log
                await _dep.AuditService.LogAsync(new UserAuditMessageDto
                {
                    UserId = user.UserId,
                    Action = "RefreshTokenSuccess",
                    OldValue = existing.Token,
                    NewValue = newRefreshToken,
                    IpAddress = GetIp(),
                    Flag = "T"
                });

                // ✅ B6: Gửi RabbitMQ UserUpdatedEvent để các service khác biết user đã hoạt động gần đây
                await _dep.UserProducer.PublishUserUpdatedAsync(new UserUpdatedEvent
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    UpdatedAt = DateTime.UtcNow
                });

                // ✅ B7: Làm mới lại cache nếu trước đó chưa có
                if (cached == null)
                {
                    await _dep.UserCache.SetAsync(MapToDto(user));
                }

                return new AuthResultDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                await _dep.AuditService.LogAsync(new UserAuditMessageDto
                {
                    UserId = 0,
                    Action = "RefreshTokenFailed",
                    OldValue = null,
                    NewValue = JsonSerializer.Serialize(new { Token = refreshToken, Message = ex.Message }),
                    IpAddress = GetIp(),
                    Flag = "T"
                });

                throw;
            }
        }

        public async Task ResendVerificationAsync(ResendVerificationRequest request, string domain)
        {
            // ✅ B1: Lấy từ cache trước, nếu không có → fallback sang DB
            var cached = await _dep.UserCache.GetByEmailAsync(request.Email);
            var user = cached != null
                ? MapToEntity(cached)
                : await _dep.UserRepository.GetByEmailAsync(request.Email)
                    ?? throw UserErrors.NotFound("For resend verification");

            // ✅ B2: Kiểm tra trạng thái xác minh và kích hoạt
            if (user.Flag == "T")
                throw UserErrors.AlreadyVerified();

            if (!user.IsActive)
                throw UserErrors.InactiveAccount();

            // ✅ B3: Tạo token xác minh mới
            var token = await _dep.UserVerificationService.CreateVerificationTokenAsync(user.UserId);

            // ✅ B4: Gửi email xác minh
            var verifyLink = $"{domain}/api/v1/auth/verify?token={token}";
            await _dep.EmailService.SendVerificationEmailAsync(user.Email, verifyLink);

            // ✅ B5: Ghi audit log (qua RabbitMQ)
            await _dep.AuditService.LogAsync(new UserAuditMessageDto
            {
                UserId = user.UserId,
                Action = "ResendVerificationEmail",
                OldValue = null,
                NewValue = JsonSerializer.Serialize(new { Email = user.Email }),
                IpAddress = GetIp(),
                Flag = "T"
            });
        }

        public async Task VerifyEmailTokenAsync(string token)
        {
            try
            {
                // ✅ B1: Lấy thông tin xác minh (token → UserId)
                var verification = await _dep.UserVerificationService.GetByTokenAsync(token)
                    ?? throw AppExceptionHelper.BadRequest("Invalid or expired token.", "INVALID_TOKEN");

                var userId = verification.UserId;

                // ✅ B2: Xác minh token
                await _dep.UserVerificationService.VerifyTokenAsync(token);

                // ✅ B3: Lấy user để xoá cache + gửi event
                var user = await _dep.UserRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    await _dep.UserCache.RemoveAsync(user.UserId, user.Email);

                    // ✅ Gửi event EmailVerified
                    await _dep.UserProducer.PublishEmailVerifiedAsync(new EmailVerifiedEvent
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        VerifiedAt = DateTime.UtcNow
                    });
                }

                // ✅ B4: Ghi log thành công
                await _dep.AuditService.LogAsync(new UserAuditMessageDto
                {
                    UserId = userId,
                    Action = "EmailVerificationSuccess",
                    OldValue = token,
                    NewValue = "Verified",
                    IpAddress = GetIp(),
                    Flag = "T"
                });
            }
            catch (Exception ex)
            {
                // ✅ Ghi log thất bại
                await _dep.AuditService.LogAsync(new UserAuditMessageDto
                {
                    UserId = 0,
                    Action = "EmailVerificationFailed",
                    OldValue = token,
                    NewValue = ex.Message,
                    IpAddress = GetIp(),
                    Flag = "F"
                });

                throw;
            }
        }

        private async Task LogLogin(int userId, bool isSuccess, string message, string? email = null)
        {
            var ip = GetIp();
            var agent = GetAgent();
            var action = isSuccess ? "LoginSuccess" : "LoginFailed";
            var newVal = new { Email = email, IP = ip, Message = message };

            // 👉 Phân tích User-Agent để lấy Device, OS, Browser
            var (device, os, browser) = UserAgentParser.Parse(agent); // Bạn có thể tạo helper này

            // ✅ Gửi message login history qua RabbitMQ
            await _dep.LoginHistoryProducer.SendAsync(new LoginHistoryMessageDto
            {
                UserId = userId,
                IsSuccess = isSuccess,
                IpAddress = ip,
                UserAgent = agent,
                Message = message,
                Device = device,
                OS = os,
                Browser = browser,
                CreatedAt = DateTime.UtcNow
            });

            // ✅ Gửi message audit qua RabbitMQ
            await _dep.AuditService.LogAsync(new UserAuditMessageDto
            {
                UserId = userId,
                Action = action,
                IpAddress = ip,
                NewValue = JsonSerializer.Serialize(newVal),
                Flag = "T"
            });
        }
    }
}