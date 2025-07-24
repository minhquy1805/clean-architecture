namespace Domain.Entities
{
    public class UserVerification
    {
        public int UserVerificationId { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = default!;
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Mở rộng: dùng để lưu IP, UserAgent hoặc custom note
        public string? Field1 { get; set; }

        public string? Field2 { get; set; }

        public string? Field3 { get; set; }
    }
}
