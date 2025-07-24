namespace Application.DTOs.Users.Message
{
    public class UserDeletedEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime DeletedAt { get; set; }
        public string Reason { get; set; } = string.Empty; // optional
    }
}
