namespace Application.DTOs.AuditLogs
{
    public class UserAuditMessageDto
    {
        public int UserId { get; set; }
        public string Action { get; set; } = null!;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IpAddress { get; set; }
        public string? Flag { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
