using System.Security.Claims;

namespace Application.Interfaces.Security
{
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Generate JWT Access Token with custom claims and expiry time.
        /// </summary>
        /// <param name="claims">Claims to include in the token</param>
        /// <param name="expiryMinutes">Token expiration time in minutes</param>
        string GenerateToken(IEnumerable<Claim> claims, int expiryMinutes);
    }
}
