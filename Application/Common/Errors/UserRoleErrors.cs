using Application.Common.Exceptions;
using Application.Common.Helpers;

namespace Application.Common.Errors
{
    public static class UserRoleErrors
    {
        public static AppException AlreadyAssigned(int userId, int roleId) =>
            AppExceptionHelper.Conflict(
                $"User (ID: {userId}) already has role (ID: {roleId}).",
                "USER_ROLE_ALREADY_ASSIGNED"
            );

        public static AppException NotAssigned(int userId, int roleId) =>
            AppExceptionHelper.BadRequest(
                $"User (ID: {userId}) does not have role (ID: {roleId}).",
                "USER_ROLE_NOT_ASSIGNED"
            );

        public static AppException InvalidUserOrRole() =>
            AppExceptionHelper.BadRequest(
                "Invalid user or role ID.",
                "INVALID_USER_OR_ROLE"
            );
    }
}
