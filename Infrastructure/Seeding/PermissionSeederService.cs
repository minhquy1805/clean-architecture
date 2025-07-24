using Application.Interfaces.Repositories.AccessControl;
using Domain.Constants;
using Domain.Entities.AccessControl;

namespace Infrastructure.Seeding
{
    public class PermissionSeederService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionSeederService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task SeedDefaultPermissionsAsync()
        {
            foreach (var permissionString in PermissionConstants.All)
            {
                var parts = permissionString.Split(":");
                var module = parts[0];
                var action = parts[1];

                var exists = await _permissionRepository.ExistsByNameAsync(permissionString);
                if (!exists)
                {
                    var permission = new Permission
                    {
                        Name = permissionString,
                        Module = module,
                        Action = action,
                        Description = $"{permissionString} permission (auto seeded)"
                    };

                    await _permissionRepository.InsertAsync(permission);
                }
            }
        }

        public async Task SeedPermissionsAsync(IEnumerable<string> permissionNames)
        {
            foreach (var permissionString in permissionNames)
            {
                var exists = await _permissionRepository.ExistsByNameAsync(permissionString);
                if (exists) continue;

                var parts = permissionString.Split(":");
                if (parts.Length != 2) continue;

                var module = parts[0];
                var action = parts[1];

                var permission = new Permission
                {
                    Name = permissionString,
                    Module = module,
                    Action = action,
                    Description = $"{permissionString} permission (custom seeded)"
                };

                await _permissionRepository.InsertAsync(permission);
            }
        }
    }
}
