using Application.Interfaces.Repositories.AccessControl;
using Domain.Constants;
using Domain.Entities.AccessControl;

namespace Infrastructure.Seeding
{
    public class RoleSeederService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleSeederService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Seed tất cả các Role mặc định từ SystemRoles.DefaultRoles nếu chưa tồn tại
        /// </summary>
        public async Task SeedDefaultRolesAsync()
        {
            foreach (var roleName in SystemRoles.DefaultRoles.Distinct())
            {
                await SeedRoleIfNotExistsAsync(roleName);
            }
        }

        /// <summary>
        /// Tạo role nếu chưa có, dùng cho seed từng role cụ thể
        /// </summary>
        public async Task SeedRoleIfNotExistsAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return;

            var exists = await _roleRepository.ExistsByNameAsync(roleName);
            if (!exists)
            {
                var role = new Role
                {
                    RoleName = roleName,
                    Description = $"{roleName} role (auto seeded)",
                    CreatedAt = DateTime.UtcNow
                };

                await _roleRepository.InsertAsync(role);
            }
        }
    }
}
