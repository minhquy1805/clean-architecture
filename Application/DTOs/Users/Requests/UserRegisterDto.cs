using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Users.Requests
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "FullName is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "FullName must be between 3 and 100 characters.")]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = default!;

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }
    }
}
