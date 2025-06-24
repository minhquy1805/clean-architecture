using Application.DTOs;
using FluentValidation;

namespace Application.Validators
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
