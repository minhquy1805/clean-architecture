using Application.DTOs;
using Application.Validators;
using FluentValidation;

namespace Application.Validators.Users
{
    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .ValidEmail(); // Tận dụng Extension bạn đã có
        }
    }
}
