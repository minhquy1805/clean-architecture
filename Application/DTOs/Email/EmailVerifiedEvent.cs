namespace Application.DTOs.Email
{
    public class EmailVerifiedEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public DateTime VerifiedAt { get; set; }
    }
}
