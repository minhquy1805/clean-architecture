using Application.Common.Exceptions;
using Application.Common.Helpers;


namespace Application.Common.Errors
{
    public static class AuthErrors
    {
        public static AppException InvalidCredentials() =>
            AppExceptionHelper.BadRequest("Invalid email or password.", "INVALID_CREDENTIALS");

        public static AppException NotVerified() =>
            AppExceptionHelper.Forbidden("Account is not verified.", "ACCOUNT_NOT_VERIFIED");

        public static AppException InactiveAccount() =>
            AppExceptionHelper.Forbidden("Account is deactivated.", "ACCOUNT_INACTIVE");

        public static AppException InvalidRefreshToken() =>
            AppExceptionHelper.BadRequest("Invalid refresh token.", "INVALID_REFRESH_TOKEN");

        public static AppException ExpiredOrRevokedToken() =>
            AppExceptionHelper.Unauthorized("Token expired or revoked.", "TOKEN_EXPIRED");

        
    }
}
