using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class LoginHistory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string LoginHistoryId { get; set; } = null!;
        public int UserId { get; set; }

        public bool IsSuccess { get; set; }

        public string IpAddress { get; set; } = string.Empty;

        public string UserAgent { get; set; } = string.Empty;

        public string Device { get; set; } = string.Empty;         // 👉 Tự động parse từ UserAgent
        public string OS { get; set; } = string.Empty;             // 👉 Hệ điều hành (Windows, iOS,...)
        public string Browser { get; set; } = string.Empty;        // 👉 Chrome, Safari,...

        public string? Message { get; set; }                       // 👉 Lý do thất bại (nếu có)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Các trường mở rộng (tuỳ chọn)
        public string? Field1 { get; set; }
        public string? Field2 { get; set; }
        public string? Field3 { get; set; }
        public string? Field4 { get; set; }
        public string? Field5 { get; set; }

        public string Flag { get; set; } = "T";
    }
}
