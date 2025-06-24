using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class ResendVerificationRequestValidator : AbstractValidator<ResendVerificationRequest>
    {
        public ResendVerificationRequestValidator()
        {
            RuleFor(x => x.Email)
                .ValidEmail(); // Reuse Extension đã có!
        }
    }
}
