namespace Application.DTOs.Seeding
{
    public class UserRoleSeedRequestDto
    {
        public string Email { get; set; } = default!;
        public string RoleName { get; set; } = default!;
        public string? PasswordIfUserNotExist { get; set; }
    }
}
