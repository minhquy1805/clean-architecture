

namespace Application.Interfaces
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hash a plain text password.
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verify a plain text password against a hashed one.
        /// </summary>
        bool VerifyPassword(string hashedPassword, string inputPassword);
    }
}
