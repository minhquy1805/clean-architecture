using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class UserAudit
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!; // MongoDB _id

        [BsonElement("userId")]
        public int UserId { get; set; }

        [BsonElement("action")]
        public string Action { get; set; } = default!;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("oldValue")]
        public string? OldValue { get; set; }

        [BsonElement("newValue")]
        public string? NewValue { get; set; }

        [BsonElement("ipAddress")]
        public string? IpAddress { get; set; }

        [BsonElement("flag")]
        public string? Flag { get; set; } = "T";

        [BsonElement("field1")]
        public string? Field1 { get; set; }

        [BsonElement("field2")]
        public string? Field2 { get; set; }

        [BsonElement("field3")]
        public string? Field3 { get; set; }

        [BsonElement("field4")]
        public string? Field4 { get; set; }

        [BsonElement("field5")]
        public string? Field5 { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
