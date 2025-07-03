

using Application.Common.Exceptions;
using Application.Common.Helpers;

namespace Application.Common.Errors
{
    public static class VerificationErrors
    {
        public static AppException TokenNotFound() =>
           AppExceptionHelper.NotFound("Verification token not found.", "VERIFICATION_TOKEN_NOT_FOUND");

        public static AppException TokenAlreadyUsed() =>
            AppExceptionHelper.BadRequest("This token has already been used.", "VERIFICATION_TOKEN_USED");

        public static AppException TokenExpired() =>
            AppExceptionHelper.BadRequest("Verification token has expired.", "VERIFICATION_TOKEN_EXPIRED");

        public static AppException UserNotFound() =>
            AppExceptionHelper.NotFound("User associated with this token was not found.", "USER_NOT_FOUND");
    }
}
