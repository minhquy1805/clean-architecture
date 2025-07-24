using Application.Interfaces.Repositories.AccessControl;
using Application.Interfaces.Repositories;
using Application.DTOs.Email;
using Application.Interfaces;
using Microsoft.Extensions.Options;
using Domain.Entities;

namespace Infrastructure.Seeding
{
    public class UserRoleSeederService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly DefaultAdminSettings _settings;

        public UserRoleSeederService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IPasswordHasher passwordHasher,
            IOptions<DefaultAdminSettings> settings)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _passwordHasher = passwordHasher;
            _settings = settings.Value;
        }


        /// <summary>
        /// Gán tài khoản minhquy073@gmail.com vào Role Admin nếu chưa có
        /// </summary>
        public async Task SeedAdminUserRoleAsync()
        {
            await SeedUserRoleAsync(_settings.Email, "Admin", _settings.Password);
        }



        /// <summary>
        /// Gán user theo email vào role cụ thể nếu chưa có
        /// </summary>
        public async Task SeedUserRoleAsync(string email, string roleName, string passwordIfUserNotExist)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Email or RoleName is null or empty.");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                var hashedPassword = _passwordHasher.HashPassword(passwordIfUserNotExist);
                var newUser = new User
                {
                    FullName = "System Admin",
                    Email = email,
                    PasswordHash = hashedPassword,
                    IsActive = true,
                    Flag = "T",
                    CreatedAt = DateTime.UtcNow
                };
                var userId = await _userRepository.InsertAsync(newUser);
                newUser.UserId = userId;
                user = newUser;
            }

            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role == null)
                throw new InvalidOperationException($"❌ Role '{roleName}' not found.");

            var assignedRoles = await _userRoleRepository.GetRoleIdsByUserIdAsync(user.UserId);
            if (!assignedRoles.Contains(role.RoleId))
            {
                await _userRoleRepository.InsertAsync(user.UserId, role.RoleId);
            }
        }
    }
}
