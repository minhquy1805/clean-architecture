

namespace Application.Interfaces.Services.Seeding
{
    public interface IRolePermissionSeederService
    {
        Task SeedPermissionsToAdminAsync();
        Task SeedPermissionsToUserAsync();
        Task SeedPermissionsToModeratorAsync();
    }
}
