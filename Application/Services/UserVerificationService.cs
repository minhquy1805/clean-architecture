using Application.Interfaces;
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
            var verification = await _verificationRepo.GetByTokenAsync(token);
            if (verification == null)
                throw new Exception("Token không tồn tại!");

            if (verification.IsUsed)
                throw new Exception("Token đã được sử dụng!");

            if (verification.ExpiryDate < DateTime.UtcNow)
                throw new Exception("Token đã hết hạn!");

            // 1️ Đánh dấu token đã dùng
            await _verificationRepo.MarkAsUsedAsync(verification.UserVerificationId);

            // 2 Update User.Flag = 'T'
            var user = await _userRepo.GetByIdAsync(verification.UserId);
            if (user == null)
                throw new Exception("User không tồn tại!");

            user.Flag = "T";
            await _userRepo.UpdateAsync(user);
        }
    }
}
