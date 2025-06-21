using Application.DTOs;


namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginRequest request);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
        Task VerifyEmailTokenAsync(string token);
        Task ResendVerificationAsync(ResendVerificationRequest request, string domain);
    }
}
