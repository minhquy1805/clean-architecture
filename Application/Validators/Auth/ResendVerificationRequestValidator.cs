using Application.DTOs.Auth.Requests;
using Application.Validators;
using FluentValidation;

namespace Application.Validators.Auth
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
