using Application.Common.Errors;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services
{
    public class UserVerificationService : IUserVerificationService
    {
        private readonly IUserVerificationRepository _verificationRepo;
        private readonly IUserRepository _userRepo;
        private readonly ITokenGenerator _tokenGenerator;

        public UserVerificationService(
            IUserVerificationRepository verificationRepo,
            IUserRepository userRepo,
            ITokenGenerator tokenGenerator)
        {
            _verificationRepo = verificationRepo;
            _userRepo = userRepo;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<string> CreateVerificationTokenAsync(int userId)
        {
            var token = _tokenGenerator.GenerateToken();
            var expiry = DateTime.UtcNow.AddHours(1);

            var verification = new UserVerification
            {
                UserId = userId,
                Token = token,
                ExpiryDate = expiry,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _verificationRepo.InsertAsync(verification);
            return token;
        }

        public async Task VerifyTokenAsync(string token)
        {
            var verification = await _verificationRepo.GetByTokenAsync(token)
                ?? throw VerificationErrors.TokenNotFound();

            if (verification.IsUsed)
                throw VerificationErrors.TokenAlreadyUsed();

            if (verification.ExpiryDate < DateTime.UtcNow)
                throw VerificationErrors.TokenExpired();

            await _verificationRepo.MarkAsUsedAsync(verification.UserVerificationId);

            var user = await _userRepo.GetByIdAsync(verification.UserId)
                ?? throw VerificationErrors.UserNotFound();

            user.Flag = "T";
            await _userRepo.UpdateAsync(user);
        }
    }
}
