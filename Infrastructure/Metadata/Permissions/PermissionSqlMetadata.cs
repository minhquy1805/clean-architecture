namespace Infrastructure.Metadata.Permissions
{
    public static class PermissionSqlMetadata
    {
        public static readonly string[] CreateFields = new[]
        {
            "Name", "Description", "Flag", "CreatedAt", "Action", "Module"
        };

        public static readonly string[] UpdateFields = new[]
        {
            "PermissionId", "Name", "Description", "Flag", "Module", "Action"
        };
    }
}
