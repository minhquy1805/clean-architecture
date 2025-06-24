using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .StrongPassword(); // Dùng lại Extension bạn đã có!
        }
    }
}
