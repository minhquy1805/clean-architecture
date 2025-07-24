namespace Application.DTOs.Users.Message
{
    public class UserPasswordChangedEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; } = "User"; // or "Admin"
    }
}
