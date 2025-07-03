// Refactored UserService based on Clean Architecture & Best Practices
// Grouped dependencies, centralized mapping & audit, DRY-ed password updates

using Application.Common.Exceptions;
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
        IRefreshTokenRepository RefreshTokenRepo
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
            if (user.Email == _mainAdminEmail && user.Role != dto.Role)
                throw UserErrors.CannotChangeMainAdminRole();
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
            var existing = await _dep.UserRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw UserErrors.EmailAlreadyUsed();

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = _dep.PasswordHasher.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Email == _mainAdminEmail ? "Admin" : "User",
                CreatedAt = DateTime.UtcNow,
                Flag = "F",
                IsActive = true
            };

            var userId = await _dep.UserRepo.InsertAsync(user);
            var token = await _dep.VerificationService.CreateVerificationTokenAsync(userId);
            var verifyLink = $"{verifyLinkBase}/api/v1/auth/verify?token={token}";

            await _dep.EmailService.SendVerificationEmailAsync(user.Email, verifyLink);
            return userId;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _dep.UserRepo.GetByEmailAsync(email);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<IEnumerable<UserDropDownDto>> GetDropDownListDataAsync()
        {
            var users = await _dep.UserRepo.GetDropDownListDataAsync();
            return users.Select(u => new UserDropDownDto { UserId = u.UserId, FullName = u.FullName });
        }

        public async Task UpdateUserAsync(UserDto dto)
        {
            var existing = await _dep.UserRepo.GetByIdAsync(dto.UserId) ?? throw UserErrors.NotFound();
            var oldValue = SerializeForAudit(existing);

            var updated = MapToEntity(dto);
            MapSensitiveFields(existing, updated);
            updated.UpdatedAt = DateTime.UtcNow;

            await _dep.UserRepo.UpdateAsync(updated);
            await LogAuditAsync(dto.UserId, "AdminUpdateUser", oldValue, SerializeForAudit(updated));
        }

        public async Task UpdateOwnProfileAsync(int userId, UpdateOwnProfileRequest dto)
        {
            var user = await _dep.UserRepo.GetByIdAsync(userId) ?? throw UserErrors.NotFound();
            if (!user.IsActive)
                throw UserErrors.InactiveAccount();

            var oldValue = SerializeForAudit(user);
            UpdateProfileFields(user, dto);
            user.UpdatedAt = DateTime.UtcNow;

            await _dep.UserRepo.UpdateAsync(user);
            await LogAuditAsync(userId, "UpdateProfile", oldValue, SerializeForAudit(user));
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _dep.UserRepo.GetByIdAsync(userId) ?? throw UserErrors.NotFound();
            if (!user.IsActive)
                throw UserErrors.InactiveAccount();

            if (!_dep.PasswordHasher.VerifyPassword(user.PasswordHash, request.CurrentPassword))
                throw UserErrors.InvalidCurrentPassword();

            await UpdatePasswordAsync(user, request.NewPassword, "ChangePassword");
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            var verification = await _dep.VerificationRepo.GetByTokenAsync(request.Token)
                ?? throw AppExceptionHelper.BadRequest("Invalid or expired token.", "INVALID_TOKEN");

            if (verification.IsUsed)
                throw AppExceptionHelper.BadRequest("Token already used.", "TOKEN_USED");

            if (verification.ExpiryDate < DateTime.UtcNow)
                throw AppExceptionHelper.BadRequest("Token expired.", "TOKEN_EXPIRED");

            var user = await _dep.UserRepo.GetByIdAsync(verification.UserId) ?? throw UserErrors.NotFound();
            if (!user.IsActive)
                throw UserErrors.InactiveAccount();

            await UpdatePasswordAsync(user, request.NewPassword, "ResetPassword");
            await _dep.VerificationRepo.MarkAsUsedAsync(verification.UserVerificationId);
        }

        public async Task ForgotPasswordAsync(string email, string verifyLinkBase)
        {
            var user = await GetUserByEmailOrThrowAsync(email);
            var token = await _dep.VerificationService.CreateVerificationTokenAsync(user.UserId);
            var link = $"{verifyLinkBase}/reset-password?token={token}";
            await _dep.EmailService.SendResetPasswordEmailAsync(email, link);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _dep.RefreshTokenRepo.DeleteByUserIdAsync(id);
        }

        public async Task SoftDeleteUserAsync(int userId)
        {
            await _dep.RefreshTokenRepo.DeleteByUserIdAsync(userId);
            await _dep.UserRepo.UpdateIsActiveAsync(userId, false);
        }

        public async Task RestoreUserAsync(int userId) => await _dep.UserRepo.UpdateIsActiveAsync(userId, true);

        private async Task<User> GetUserByEmailOrThrowAsync(string email)
        {
            var user = await _dep.UserRepo.GetByEmailAsync(email);
            if (user == null || !user.IsActive)
                throw UserErrors.EmailNotFoundOrInactive();
            return user;
        }

        private async Task UpdatePasswordAsync(User user, string newPassword, string action)
        {
            var oldValue = SerializePasswordForAudit("OldPasswordHash", user.PasswordHash);
            user.PasswordHash = _dep.PasswordHasher.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _dep.UserRepo.UpdateAsync(user);
            var newValue = SerializePasswordForAudit("NewPasswordHash", user.PasswordHash);
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
