using Application.Common.Exceptions;

namespace Application.Common.Helpers
{
    public static class AppExceptionHelper
    {
        public static AppException BadRequest(string message, string errorCode = "BAD_REQUEST", string? details = null) =>
            new AppException(message, 400, errorCode, details);

        public static AppException Unauthorized(string message = "Unauthorized", string errorCode = "UNAUTHORIZED", string? details = null) =>
            new AppException(message, 401, errorCode, details);

        public static AppException Forbidden(string message = "Forbidden", string errorCode = "FORBIDDEN", string? details = null) =>
            new AppException(message, 403, errorCode, details);

        public static AppException NotFound(string message = "Not found", string errorCode = "NOT_FOUND", string? details = null) =>
            new AppException(message, 404, errorCode, details);

        public static AppException Conflict(string message = "Conflict", string errorCode = "CONFLICT", string? details = null) =>
            new AppException(message, 409, errorCode, details);

        public static AppException Internal(string message = "Internal server error", string errorCode = "INTERNAL_ERROR", string? details = null) =>
            new AppException(message, 500, errorCode, details);

        public static AppException Custom(string message, int statusCode, string errorCode, string? details = null) =>
            new AppException(message, statusCode, errorCode, details);

        public static AppException FromException(Exception ex, string errorCode = "INTERNAL_EXCEPTION") =>
            new AppException(ex.Message, ex, 500, errorCode, ex.StackTrace);
    }
}
