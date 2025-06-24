using Application.DTOs;
using FluentValidation;


namespace Application.Validators
{
    public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterDtoValidator() // 👉 Constructor phải có {}
        {
            RuleFor(x => x.FullName).ValidName();
            RuleFor(x => x.Email).ValidEmail();
            RuleFor(x => x.Password).StrongPassword();
            RuleFor(x => x.PhoneNumber).ValidPhone();
        }
    }
}
