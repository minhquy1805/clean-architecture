using Application.DTOs.Users.Requests;
using Application.Validators;
using FluentValidation;

namespace Application.Validators.Users
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.");

            RuleFor(x => x.NewPassword)
                .StrongPassword(); // Reuse Extension
        }
    }
}
