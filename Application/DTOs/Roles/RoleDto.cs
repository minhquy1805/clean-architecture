namespace Application.DTOs.Roles
{
    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string? Description { get; set; }
        public string Flag { get; set; } = "T";
        public DateTime? CreatedAt { get; set; }
    }
}
