namespace Application.DTOs.LoginHistories
{
    public class LoginHistoryDto
    {
        public int LoginId { get; set; }
        public int UserId { get; set; }
        public bool IsSuccess { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
