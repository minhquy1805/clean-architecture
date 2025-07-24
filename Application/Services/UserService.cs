// Refactored UserService based on Clean Architecture & Best Practices
// Grouped dependencies, centralized mapping & audit, DRY-ed password updates
using Application.Common.Errors;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Mappings;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Application.Common.Helpers;
using Application.Services.Abstract;
using Application.DTOs.Users.Requests;
using Application.DTOs.Users.Responses;
using Application.DTOs.Users.Filters;
using Domain.Constants;
using Application.Interfaces.Services.AccessControl;
using Application.Interfaces.Redis.Caching;
using Application.Interfaces.Messaging.Producers;
using Application.DTOs.Users.Message;
using Application.DTOs.AuditLogs;
using Application.DTOs;

namespace Application.Services
{
    public record UserServiceDependencies(
        IUserRepository UserRepo,
        IUserVerificationService VerificationService,
        IEmailService EmailService,
        IPasswordHasher PasswordHasher,
        IConfiguration Configuration,
        IUserVerificationRepository VerificationRepo,
        IUserAuditRepository AuditRepo,
        IHttpContextAccessor HttpContextAccessor,
        IRefreshTokenRepository RefreshTokenRepo,
        IRoleService RoleService,
        IUserRoleService UserRoleService,
        IUserCacheService UserCache,
        IRabbitMqUserMessageProducer UserProducer,
        IAuditService AuditService
    );

    public partial class UserService : BasePagingFilterService<UserDto, User, UserFilterDto>, IUserService
    {
        private readonly UserServiceDependencies _dep;
        private readonly string _mainAdminEmail;

        public UserService(UserServiceDependencies dep)
            : base(dep.UserRepo)
        {
            _dep = dep;
            _mainAdminEmail = _dep.Configuration["MainAdminEmail"] ?? "admin@example.com";
        }

        protected override UserDto MapToDto(User entity) => UserMapper.ToDto(entity);
        protected override User MapToEntity(UserDto dto) => UserMapper.ToEntity(dto);
        protected override int GetDtoId(UserDto dto) => dto.UserId;

        protected override async Task ValidateBeforeUpdate(UserDto dto)
        {
            var user = await _dep.UserRepo.GetByIdAsync(dto.UserId) ?? throw UserErrors.NotFound();

            var other = await _dep.UserRepo.GetByEmailAsync(dto.Email);
            if (other != null && other.UserId != dto.UserId)
                throw UserErrors.EmailAlreadyUsed();
        }

        protected override Task ValidateBeforeDelete(int id)
        {
            if (id == GetCurrentUserId())
                throw UserErrors.CannotDeleteYourself();
            return Task.CompletedTask;
        }

        protected override async Task LogAuditAsync(int userId, string action, string? oldValue, string? newValue)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Audit action must be specified.");

            var audit = new UserAudit
            {
                UserId = userId,
                Action = action.Trim(),
                OldValue = oldValue ?? string.Empty,
                NewValue = newValue ?? string.Empty,
                IpAddress = GetIpAddress()
            };

            await _dep.AuditRepo.InsertAsync(audit);
        }

        public override Task<int> GetRecordCountWhereDynamicAsync(UserFilterDto filter)
        {
            return _dep.UserRepo.GetRecordCountWhereDynamicAsync(filter);
        }

        public override async Task<IEnumerable<UserDto>> SelectSkipAndTakeWhereDynamicAsync(UserFilterDto filter)
        {
            var users = await _dep.UserRepo.SelectSkipAndTakeWhereDynamicAsync(filter);
            return users.Select(MapToDto);
        }

        protected override string[] GetAllowedSortFields() =>
            new[] { "UserId", "FullName", "Email", "Role", "CreatedAt", "UpdatedAt", "IsActive", "Flag" };

        public async Task<int> RegisterUserAsync(UserRegisterDto dto, string verifyLinkBase)
        {
            // 🔍 B1: Check email đã tồn tại (ưu tiên cache nếu có)
            var cachedDto = await _dep.UserCache.GetByEmailAsync(dto.Email);
            if (cachedDto != null)
                throw UserErrors.EmailAlreadyUsed();

            var existing = await _dep.UserRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw UserErrors.EmailAlreadyUsed();

            // 🏗️ B2: Tạo user mới
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = _dep.PasswordHasher.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                Flag = "F", // ❌ Chưa xác thực
                IsActive = true
            };

            var userId = await _dep.UserRepo.InsertAsync(user);

            // 🔐 B3: Gán role (Admin nếu là email chính)
            var roleName = dto.Email == _mainAdminEmail ? SystemRoles.Admin : SystemRoles.User;
            var role = await _dep.RoleService.GetByNameAsync(roleName)
                       ?? throw UserRoleErrors.InvalidUserOrRole();

            await _dep.UserRoleService.AddRoleToUserAsync(userId, role.RoleId);

            // 📩 B4: Gửi email xác thực
            var token = await _dep.VerificationService.CreateVerificationTokenAsync(userId);
            var verifyLink = $"{verifyLinkBase}/api/v1/auth/verify?token={token}";
            await _dep.EmailService.SendVerificationEmailAsync(user.Email, verifyLink);

            // 📦 B5: Gửi event RabbitMQ (UserRegistered)
            await _dep.UserProducer.PublishUserRegisteredAsync(new UserRegisteredEvent
            {
                UserId = userId,
                Email = user.Email,
                FullName = user.FullName,
                RegisteredAt = user.CreatedAt,
                Role = roleName,
                PhoneNumber = user.PhoneNumber!
            });

            // 📝 B6: Ghi audit log
            await _dep.AuditService.LogAsync(new UserAuditMessageDto
            {
                UserId = userId,
                Action = "RegisterUser",
                NewValue = JsonSerializer.Serialize(user),
                IpAddress = GetIpAddress(),
                Flag = "T"
            });

            return userId;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var cached = await _dep.UserCache.GetByEmailAsync(email);
            if (cached != null) return cached;

            var user = await _dep.UserRepo.GetByEmailAsync(email);
            if (user == null) return null;

            var dto = MapToDto(user);
            await _dep.UserCache.SetAsync(dto);
            return dto;
        }

        public async Task<IEnumerable<UserDropDownDto>> GetDropDownListDataAsync()
        {
            var users = await _dep.UserRepo.GetDropDownListDataAsync();
            return users.Select(u => new UserDropDownDto { UserId = u.UserId, FullName = u.FullName });
        }

        public async Task UpdateUserAsync(UserDto dto)
        {
            // ✅ Bước 1: Lấy từ cache trước để tăng hiệu suất
            var cached = await _dep.UserCache.GetByIdAsync(dto.UserId);

            // ✅ Bước 2: Nếu cache có → map sang Entity; nếu không có → lấy từ DB
            var existingEntity = cached != null
                ? MapToEntity(cached)
                : await _dep.UserRepo.GetByIdAsync(dto.UserId) ?? throw UserErrors.NotFound();

            // ✅ Bước 3: Dùng DTO hiện có (từ cache hoặc DB) để làm oldValue cho audit log
            var existing = cached ?? MapToDto(existingEntity);
            var oldValue = SerializeForAudit(existing);

            // ✅ Bước 4: Map DTO mới vào entity mới để cập nhật
            var updated = MapToEntity(dto);

            // ✅ Bước 5: Giữ lại các field không được ghi đè (PasswordHash, CreatedAt,...)
            MapSensitiveFields(existingEntity, updated);
            updated.UpdatedAt = DateTime.UtcNow;

            // ✅ Bước 6: Cập nhật vào DB
            await _dep.UserRepo.UpdateAsync(updated);

            // ✅ Bước 7: Ghi log audit cho cập nhật
            await LogAuditAsync(dto.UserId, "AdminUpdateUser", oldValue, SerializeForAudit(updated));

            // ✅ Bước 8: Xoá cache cũ (theo UserId và Email)
            await _dep.UserCache.RemoveAsync(dto.UserId, dto.Email);

            // ✅ Bước 9: Set lại cache mới sau khi cập nhật
            await _dep.UserCache.SetAsync(MapToDto(updated));

            // ✅ Bước 10: Gửi RabbitMQ event
            await _dep.UserProducer.PublishUserUpdatedAsync(new UserUpdatedEvent
            {
                UserId = updated.UserId,
                Email = updated.Email,
                FullName = updated.FullName,
                UpdatedAt = updated.UpdatedAt!.Value
            });

        }

        public async Task UpdateOwnProfileAsync(int userId, UpdateOwnProfileRequest dto)
        {
            var user = await _dep.UserRepo.GetByIdAsync(userId)
                ?? throw UserErrors.NotFound();

            if (!user.IsActive)
                throw UserErrors.InactiveAccount();

            var oldValue = SerializeForAudit(user);

            // ✅ Cập nhật trường
            UpdateProfileFields(user, dto);
            user.UpdatedAt = DateTime.UtcNow;

            // ✅ Cập nhật DB
            await _dep.UserRepo.UpdateAsync(user);

            // ✅ Ghi Audit Log
            await _dep.AuditService.LogAsync(new UserAuditMessageDto
            {
                UserId = userId,
                Action = "UpdateOwnProfile",
                OldValue = oldValue,
                NewValue = SerializeForAudit(user),
                IpAddress = GetIpAddress(),
                Flag = "T"
            });

            // ✅ Gửi RabbitMQ event
            await _dep.UserProducer.PublishUserUpdatedAsync(new UserUpdatedEvent
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                UpdatedAt = user.UpdatedAt!.Value
            });

            // ✅ Xoá + update cache
            await _dep.UserCache.RemoveAsync(user.UserId, user.Email);
            await _dep.UserCache.SetAsync(MapToDto(user));
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            // ✅ B1: Lấy user từ DB
            var user = await _dep.UserRepo.GetByIdAsync(userId)
                ?? throw UserErrors.NotFound();

            // ✅ B2: Kiểm tra trạng thái & mật khẩu hiện tại
            if (!user.IsActive)
                throw UserErrors.InactiveAccount();

            if (!_dep.PasswordHasher.VerifyPassword(user.PasswordHash, request.CurrentPassword))
                throw UserErrors.InvalidCurrentPassword();

            var oldHash = user.PasswordHash;

            // ✅ B3: Cập nhật mật khẩu
            await UpdatePasswordAsync(user, request.NewPassword, "ChangePassword");

            // ✅ B4: Gửi event RabbitMQ
            await _dep.UserProducer.PublishPasswordChangedAsync(new UserPasswordChangedEvent
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                ChangedAt = DateTime.UtcNow
            });

            // ✅ B5: Ghi log audit
            await _dep.AuditService.LogAsync(new UserAuditMessageDto
            {
                UserId = user.UserId,
                Action = "ChangePassword",
                OldValue = oldHash,
                NewValue = user.PasswordHash,
                IpAddress = GetIpAddress(),
                Flag = "T"
            });

            // ✅ B6: Làm sạch + cập nhật lại cache
            await _dep.UserCache.RemoveAsync(user.UserId, user.Email);
            await _dep.UserCache.SetAsync(MapToDto(user));
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            // ✅ B1: Xác minh token hợp lệ chưa
            var verification = await _dep.VerificationRepo.GetByTokenAsync(request.Token)
                ?? throw AppExceptionHelper.BadRequest("Invalid or expired token.", "INVALID_TOKEN");

            if (verification.IsUsed)
                throw AppExceptionHelper.BadRequest("Token already used.", "TOKEN_USED");

            if (verification.ExpiryDate < DateTime.UtcNow)
                throw AppExceptionHelper.BadRequest("Token expired.", "TOKEN_EXPIRED");

            var user = await _dep.UserRepo.GetByIdAsync(verification.UserId)
                ?? throw UserErrors.NotFound();

            if (!user.IsActive)
                throw UserErrors.InactiveAccount();

            // ✅ B2: Cập nhật mật khẩu thông qua hàm chuẩn
            await UpdatePasswordAsync(user, request.NewPassword, "ResetPassword");

            // ✅ B3: Đánh dấu token đã dùng
            await _dep.VerificationRepo.MarkAsUsedAsync(verification.UserVerificationId);

            // ✅ B4: Clear + update cache
            await _dep.UserCache.RemoveAsync(user.UserId, user.Email);
            await _dep.UserCache.SetAsync(MapToDto(user));

            // ✅ B5: Gửi RabbitMQ Event nếu cần (nếu chưa có trong UpdatePasswordAsync)
            await _dep.UserProducer.PublishPasswordChangedAsync(new UserPasswordChangedEvent
            {
                UserId = user.UserId,
                Email = user.Email,
                ChangedAt = DateTime.UtcNow
            });
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request, string domain)
        {
            // ✅ B1: Lấy user từ cache trước → fallback sang DB nếu không có
            var cached = await _dep.UserCache.GetByEmailAsync(request.Email);
            var user = cached != null
                ? MapToEntity(cached)
                : await _dep.UserRepo.GetByEmailAsync(request.Email)
                    ?? throw UserErrors.EmailNotFoundOrInactive();

            // ✅ B2: Kiểm tra trạng thái
            if (!user.IsActive)
                throw UserErrors.InactiveAccount();

            // ✅ B3: Tạo token reset mật khẩu
            var token = await _dep.VerificationService.CreateVerificationTokenAsync(user.UserId);
            var resetLink = $"{domain}/reset-password?token={token}";

            // ✅ B4: Gửi email
            await _dep.EmailService.SendResetPasswordEmailAsync(user.Email, resetLink);

            // ✅ B5: Ghi audit log
            await _dep.AuditService.LogAsync(new UserAuditMessageDto
            {
                UserId = user.UserId,
                Action = "ForgotPassword",
                OldValue = null,
                NewValue = JsonSerializer.Serialize(new { Email = user.Email, Link = resetLink }),
                IpAddress = GetIpAddress(),
                Flag = "T"
            });

            // ✅ B6 (tuỳ chọn): Cập nhật cache nếu user lấy từ DB
            if (cached == null)
                await _dep.UserCache.SetAsync(MapToDto(user));
        }

        public async Task DeleteUserAsync(int id)
        {
            await _dep.RefreshTokenRepo.DeleteByUserIdAsync(id);
        }

        public async Task SoftDeleteUserAsync(int userId)
        {
            // ✅ B1: Xoá refresh token của user (bảo mật)
            await _dep.RefreshTokenRepo.DeleteByUserIdAsync(userId);

            // ✅ B2: Lấy thông tin user từ cache hoặc DB
            var cached = await _dep.UserCache.GetByIdAsync(userId);
            var user = cached != null
                ? MapToEntity(cached)
                : await _dep.UserRepo.GetByIdAsync(userId) ?? throw UserErrors.NotFound();

            // ✅ B3: Kiểm tra trạng thái
            if (!user.IsActive)
                throw UserErrors.AlreadyDeactivated();

            // ✅ B4: Ghi audit log trước khi thay đổi
            await LogAuditAsync(userId, "SoftDeleteUser", SerializeForAudit(user), "IsActive=false");

            // ✅ B5: Cập nhật trạng thái trong DB
            await _dep.UserRepo.UpdateIsActiveAsync(userId, false);

            // ✅ B6: Clear cache
            await _dep.UserCache.RemoveAsync(userId, user.Email);
        }

        public async Task RestoreUserAsync(int userId)
        {
            // ✅ B1: Lấy từ cache trước để tối ưu
            var cached = await _dep.UserCache.GetByIdAsync(userId);

            // ✅ B2: Nếu có cache thì map lại sang entity, nếu không thì lấy từ DB
            var user = cached != null
                ? MapToEntity(cached)
                : await _dep.UserRepo.GetByIdAsync(userId) ?? throw UserErrors.NotFound();

            // ✅ B3: Kiểm tra trạng thái hiện tại
            if (user.IsActive)
                throw UserErrors.AlreadyActive();

            // ✅ B4: Ghi log audit
            await LogAuditAsync(userId, "RestoreUser", SerializeForAudit(user), "IsActive=true");

            // ✅ B5: Cập nhật trạng thái vào DB
            await _dep.UserRepo.UpdateIsActiveAsync(userId, true);

            // ✅ B6: Clear + update lại cache
            await _dep.UserCache.RemoveAsync(userId, user.Email);
            user.IsActive = true;
            await _dep.UserCache.SetAsync(MapToDto(user));
        }

        private async Task UpdatePasswordAsync(User user, string newPassword, string action)
        {
            // ✅ B1: Ghi lại giá trị cũ (hash mật khẩu)
            var oldValue = SerializePasswordForAudit("OldPasswordHash", user.PasswordHash);

            // ✅ B2: Cập nhật mật khẩu và thời gian
            user.PasswordHash = _dep.PasswordHasher.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // ✅ B3: Ghi lại giá trị mới
            var newValue = SerializePasswordForAudit("NewPasswordHash", user.PasswordHash);

            // ✅ B4: Cập nhật DB
            await _dep.UserRepo.UpdateAsync(user);

            // ✅ B5: Gửi RabbitMQ event
            await _dep.UserProducer.PublishPasswordChangedAsync(new UserPasswordChangedEvent
            {
                UserId = user.UserId,
                Email = user.Email,
                ChangedAt = user.UpdatedAt.Value
            });

            // ✅ B6: Ghi log audit
            await LogAuditAsync(user.UserId, action, oldValue, newValue);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _dep.HttpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        private string GetIpAddress() => _dep.HttpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "";

        private void MapSensitiveFields(User source, User target)
        {
            target.PasswordHash = source.PasswordHash;
            target.CreatedAt = source.CreatedAt;
        }

        private void UpdateProfileFields(User user, UpdateOwnProfileRequest dto)
        {
            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.AvatarUrl = dto.AvatarUrl;
            user.DateOfBirth = dto.DateOfBirth;
            user.Gender = dto.Gender;
        }

        private new string SerializeForAudit(object obj) => JsonSerializer.Serialize(obj, _auditOptions);
        private new string SerializePasswordForAudit(string label, string hash) => $"{label}: {hash}";

        private static readonly JsonSerializerOptions _auditOptions = new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };
    }
}
