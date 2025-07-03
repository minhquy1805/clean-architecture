
namespace Infrastructure.Metadata.Users
{
    public static class UserSqlMetadata
    {
        public static readonly string[] CreateFields = new[]
        {
            "FullName", "Email", "PasswordHash", "Role",
            "AvatarUrl", "PhoneNumber", "DateOfBirth", "Gender", "IsActive",
            "Field1", "Field2", "Field3", "Field4", "Field5", "Flag"
        };

        public static readonly string[] UpdateFields = new[]
       {
            "UserId", "FullName", "Email", "PasswordHash", "Role",
            "AvatarUrl", "PhoneNumber", "DateOfBirth", "Gender", "IsActive",
            "Field1", "Field2", "Field3", "Field4", "Field5", "Flag", "LastLoginAt"
        };
    }
}
