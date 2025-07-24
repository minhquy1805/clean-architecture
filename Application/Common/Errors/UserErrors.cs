using Application.Common.Exceptions;
using Application.Common.Helpers;

namespace Application.Common.Errors
{
    public static class UserErrors
    {
        public static AppException NotFound(string? details = null) =>
            AppExceptionHelper.NotFound("User not found.", "USER_NOT_FOUND", details);

        public static AppException EmailAlreadyUsed() =>
            AppExceptionHelper.Conflict("Email is already in use.", "EMAIL_CONFLICT");

        public static AppException CannotChangeMainAdminRole() =>
            AppExceptionHelper.Forbidden("Cannot change role of main admin.", "ADMIN_ROLE_PROTECTED");

        public static AppException CannotDeleteYourself() =>
            AppExceptionHelper.Forbidden("You cannot delete your own account.", "DELETE_SELF_FORBIDDEN");

        public static AppException InvalidCurrentPassword() =>
            AppExceptionHelper.BadRequest("Current password is incorrect.", "INVALID_CURRENT_PASSWORD");

        public static AppException AlreadyVerified() =>
            AppExceptionHelper.BadRequest("Account is already verified.", "ALREADY_VERIFIED");

        public static AppException EmailNotFoundOrInactive() =>
            AppExceptionHelper.BadRequest("Email not found or account is inactive.", "EMAIL_INVALID_OR_INACTIVE");

        public static AppException InactiveAccount() =>
            AppExceptionHelper.Forbidden("Account is deactivated.", "ACCOUNT_INACTIVE");

        public static AppException AlreadyActive() =>
            AppExceptionHelper.BadRequest("Account is already active.", "ACCOUNT_ALREADY_ACTIVE");

        public static AppException AlreadyDeactivated() =>
            AppExceptionHelper.BadRequest("Account is already deactivated.", "ACCOUNT_ALREADY_DEACTIVATED");
    }
}
