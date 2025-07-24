namespace Application.DTOs.Users.Message
{
    public class UserRegisteredEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime RegisteredAt { get; set; }
    }
}
