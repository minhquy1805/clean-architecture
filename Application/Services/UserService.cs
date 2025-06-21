using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;


namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserVerificationService _userVerificationService;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IUserVerificationRepository _verificationRepo;

        public UserService(
            IUserRepository userRepository,
            IUserVerificationService userVerificationService,
            IEmailService emailService,
            IPasswordHasher passwordHasher,
            IConfiguration configuration,
            IUserVerificationRepository verificationRepo)
        {
            _userRepository = userRepository;
            _userVerificationService = userVerificationService;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _verificationRepo = verificationRepo;
        }


        // ✅ Register User: Full flow
        public async Task<int> RegisterUserAsync(UserRegisterDto dto, string verifyLinkBase)
        {
            // 1️ Check email tồn tại
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new Exception("Email is already registered!");

            // 2️ Hash password
            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            // 3️ Map DTO -> Entity, Flag = "F"
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Email == "minhquy073@gmail.com" ? "Admin" : "User",
                CreatedAt = DateTime.UtcNow,
                Flag = "F"
            };

            // 4️ Insert
            var userId = await _userRepository.InsertAsync(user);

            // 5 Create Token
            var token = await _userVerificationService.CreateVerificationTokenAsync(userId);

            // 6️ Build Verify Link from base
            var verifyLink = $"{verifyLinkBase}/api/v1/auth/verify?token={token}";

            // 7️ Send Email
            await _emailService.SendVerificationEmailAsync(user.Email, verifyLink);

            return userId;
        }

        // ✅ Get by Id
        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        // ✅ Get All
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        // ✅ Paging Filter
        public async Task<IEnumerable<UserDto>> SelectSkipAndTakeWhereDynamicAsync(string whereCondition, int start, int rows, string sortBy)
        {
            var users = await _userRepository.SelectSkipAndTakeWhereDynamicAsync(whereCondition, start, rows, sortBy);
            return users.Select(MapToDto);
        }

        public async Task<int> GetRecordCountWhereDynamicAsync(string whereCondition)
        {
            return await _userRepository.GetRecordCountWhereDynamicAsync(whereCondition);
        }

        // ✅ Get by Email
        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? MapToDto(user) : null;
        }

        // ✅ Dropdown
        public async Task<IEnumerable<UserDropDownDto>> GetDropDownListDataAsync()
        {
            var users = await _userRepository.GetDropDownListDataAsync();
            return users.Select(u => new UserDropDownDto
            {
                UserId = u.UserId,
                FullName = u.FullName
            });
        }

        // ✅ Update
        public async Task UpdateUserAsync(UserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null) throw new Exception("User not found!");

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.AvatarUrl = dto.AvatarUrl;
            user.Gender = dto.Gender;
            user.DateOfBirth = dto.DateOfBirth;
            user.Role = dto.Role;
            user.Flag = dto.Flag;
            user.IsActive = dto.IsActive;
            user.Field1 = dto.Field1;
            user.Field2 = dto.Field2;
            user.Field3 = dto.Field3;
            user.Field4 = dto.Field4;
            user.Field5 = dto.Field5;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        // ✅ Update User Profile
        public async Task UpdateOwnProfileAsync(int userId, UserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found!");

            // Only update fields that the user is allowed to change.
            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.AvatarUrl = dto.AvatarUrl;
            user.DateOfBirth = dto.DateOfBirth;
            user.Gender = dto.Gender;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        // ✅ Change Password
        public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found!");

            if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                throw new Exception("Current password is incorrect.");

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        // Reset password request
        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            // Reuse Verify logic
            var verification = await _verificationRepo.GetByTokenAsync(request.Token);
            if (verification == null)
                throw new Exception("Invalid or expired token.");

            if (verification.IsUsed)
                throw new Exception("Token already used.");

            if (verification.ExpiryDate < DateTime.UtcNow)
                throw new Exception("Token expired.");

            // Update password
            var user = await _userRepository.GetByIdAsync(verification.UserId);
            if (user == null) throw new Exception("User not found!");

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // Mark token used
            await _verificationRepo.MarkAsUsedAsync(verification.UserVerificationId);
        }

        public async Task ForgotPasswordAsync(string email, string verifyLinkBase)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new Exception("Email not found.");

            // Tạo 1 verification token mới
            var token = await _userVerificationService.CreateVerificationTokenAsync(user.UserId);

            // Build link reset
            var link = $"{verifyLinkBase}/reset-password?token={token}";

            // Gửi email
            await _emailService.SendResetPasswordEmailAsync(email, link);
        }


        // ✅ Delete
        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        // ✅ Helper: Map Entity -> DTO
        private static UserDto MapToDto(User user) =>
            new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                AvatarUrl = user.AvatarUrl,
                Field1 = user.Field1,
                Field2 = user.Field2,
                Field3 = user.Field3,
                Field4 = user.Field4,
                Field5 = user.Field5,
                Flag = user.Flag,
                IsActive = user.IsActive
            };
    }


}
