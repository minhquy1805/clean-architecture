namespace Domain.Entities.AccessControl
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string? Description { get; set; }
        public string Flag { get; set; } = "T";
        public DateTime? CreatedAt { get; set; }
    }
}
