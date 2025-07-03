namespace Domain.Entities
{
    public class RefreshToken
    {
        public int TokenId { get; set; }  // PK trong DB
        public int UserId { get; set; }
        public string Token { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public string Flag { get; set; } = "T";

        // Extra fields
        public string? Field1 { get; set; }
        public string? Field2 { get; set; }
        public string? Field3 { get; set; }
        public string? Field4 { get; set; }
        public string? Field5 { get; set; }
    }
}
