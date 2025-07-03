using Application.Interfaces.Security;


namespace Infrastructure.Services.Auth
{
    public class TokenGenerator : ITokenGenerator
    {
        public string GenerateToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
