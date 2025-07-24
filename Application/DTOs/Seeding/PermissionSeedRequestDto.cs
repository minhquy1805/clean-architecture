namespace Application.DTOs.Seeding
{
    public class PermissionSeedRequestDto
    {
        public string RoleName { get; set; } = default!;
        public List<string> Permissions { get; set; } = new();
    }
}
