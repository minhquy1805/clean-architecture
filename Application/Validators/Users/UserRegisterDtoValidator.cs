using Application.DTOs.Users.Requests;
using Application.Validators;
using FluentValidation;


namespace Application.Validators.Users
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
