namespace Domain.Entities.AccessControl
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public string Name { get; set; } = null!;        // Ex: "User:Create"
        public string? Module { get; set; }              // Ex: "User"
        public string? Action { get; set; }              // Ex: "Create"
        public string? Description { get; set; }
        public string Flag { get; set; } = "T";
        public DateTime? CreatedAt { get; set; }
    }
}
