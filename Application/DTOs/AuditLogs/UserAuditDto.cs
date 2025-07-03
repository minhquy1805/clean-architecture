namespace Application.DTOs.AuditLogs
{
    public class UserAuditDto
    {
        public int AuditId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = default!;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IpAddress { get; set; }
        public string? Flag { get; set; }
        public string? Field1 { get; set; }
        public string? Field2 { get; set; }
        public string? Field3 { get; set; }
        public string? Field4 { get; set; }
        public string? Field5 { get; set; }
        public DateTime CreatedAt { get; set; }

        public Dictionary<string, object>? OldObject { get; set; }
        public Dictionary<string, object>? NewObject { get; set; }
    }
}
