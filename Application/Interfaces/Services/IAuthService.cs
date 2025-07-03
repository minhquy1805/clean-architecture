using Application.DTOs.Auth;
using Application.DTOs.Auth.Requests;


namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginRequest request);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
        Task VerifyEmailTokenAsync(string token);
        Task ResendVerificationAsync(ResendVerificationRequest request, string domain);
    }
}
