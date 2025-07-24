

namespace Application.DTOs.Users.Message
{
    public class UserUpdatedEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = "System"; // optional
    }
}
