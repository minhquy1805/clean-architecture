using Application.Common.Exceptions;
using Application.Common.Helpers;

namespace Application.Common.Errors
{
    public static class RolePermissionErrors
    {
        public static AppException AlreadyAssigned(int roleId, int permissionId) =>
            AppExceptionHelper.Conflict(
                $"Role (ID: {roleId}) already has permission (ID: {permissionId}).",
                "ROLE_PERMISSION_ALREADY_ASSIGNED"
            );

        public static AppException NotAssigned(int roleId, int permissionId) =>
            AppExceptionHelper.BadRequest(
                $"Role (ID: {roleId}) does not have permission (ID: {permissionId}).",
                "ROLE_PERMISSION_NOT_ASSIGNED"
            );

        public static AppException InvalidRoleOrPermission() =>
            AppExceptionHelper.BadRequest(
                "Invalid role or permission ID.",
                "INVALID_ROLE_OR_PERMISSION"
            );
    }
}
