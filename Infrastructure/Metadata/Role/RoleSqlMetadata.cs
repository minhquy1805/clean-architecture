namespace Infrastructure.Metadata.Role
{
    public static class RoleSqlMetadata
    {
        public static readonly string[] CreateFields = new[]
        {
            "RoleName",
            "Description",
            "Flag",
            "CreatedAt"
        };

        public static readonly string[] UpdateFields = new[]
        {
            "RoleId",
            "RoleName",
            "Description",
            "Flag"
        };
    }
}
