using FluentValidation;


namespace Application.Validators
{
    /// <summary>
    /// Shared extension methods for FluentValidation rules.
    /// Use to apply consistent validation across multiple DTOs.
    /// </summary>
    public static class FluentValidationExtensions
    {
        /// <summary>
        /// Validate a name: not empty, 3-100 chars.
        /// </summary>
        public static IRuleBuilder<T, string> ValidName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Name is required.")
                .Length(3, 100).WithMessage("Name must be between 3 and 100 characters.");
        }

        /// <summary>
        /// Validate an email: not empty, valid format.
        /// </summary>
        public static IRuleBuilder<T, string> ValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }

        /// <summary>
        /// Validate a strong password: min 8 chars, at least 1 uppercase, 1 lowercase, 1 number, 1 special char.
        /// </summary>
        public static IRuleBuilder<T, string> StrongPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        }

        /// <summary>
        /// Validate phone number: optional but if present must match pattern.
        /// </summary>
        public static IRuleBuilder<T, string?> ValidPhone<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .Matches(@"^\+?[0-9]{7,15}$")
                .When(phone => !string.IsNullOrEmpty(phone as string))
                .WithMessage("Invalid phone number format. Use digits only, optionally starting with '+'.");
        }
    }
}
