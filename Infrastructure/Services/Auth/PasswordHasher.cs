using Application.Interfaces;


namespace Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            if (string.IsNullOrWhiteSpace(inputPassword))
                throw new ArgumentNullException(nameof(inputPassword), "⚠ inputPassword is null or empty.");

            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new ArgumentNullException(nameof(hashedPassword), "⚠ hashedPassword is null or empty.");

            return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        }
    }
}
