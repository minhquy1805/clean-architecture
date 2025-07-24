namespace Application.DTOs.LoginHistories
{
    public class LoginHistoryMessageDto
    {
        public int UserId { get; set; }
        public bool IsSuccess { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Message { get; set; }
        public string? Device { get; set; }
        public string? OS { get; set; }
        public string? Browser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
