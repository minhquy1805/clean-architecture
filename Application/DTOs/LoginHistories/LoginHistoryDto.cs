namespace Application.DTOs.LoginHistories
{
    public class LoginHistoryDto
    {
        public string LoginHistoryId { get; set; } = null!;

        public int UserId { get; set; }

        public bool IsSuccess { get; set; } 

        public string IpAddress { get; set; } = string.Empty;

        public string UserAgent { get; set; } = string.Empty;

        public string Device { get; set; } = string.Empty;

        public string OS { get; set; } = string.Empty;

        public string Browser { get; set; } = string.Empty;

        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
