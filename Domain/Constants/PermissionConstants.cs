public static class PermissionConstants
{
    public static readonly Dictionary<string, string[]> DefaultPermissions = new()
    {
        // --- Module quản trị ---
        { "User", new[] { "View", "Create", "Update", "Delete" } },
        { "Role", new[] { "Manage" } },
        { "Permission", new[] { "Manage" } },
        { "Dashboard", new[] { "View" } },
        { "AuditLog", new[] { "View" } },
        { "LoginHistory", new[] { "View" } },

        // --- Module người dùng (self) ---
        { "UserSelf", new[] { "Update", "ChangePassword" } }, // ✅ nếu bạn muốn policy riêng
    };

    // ✅ Tất cả quyền: User:View, Role:Manage, UserSelf:Update, ...
    public static IEnumerable<string> All =>
        DefaultPermissions.SelectMany(kv =>
            kv.Value.Select(action => $"{kv.Key}:{action}")
        );

    // ✅ Trả về quyền theo module
    public static IEnumerable<string> GetPermissionsForModule(string module)
    {
        return DefaultPermissions.TryGetValue(module, out var actions)
            ? actions.Select(action => $"{module}:{action}")
            : Enumerable.Empty<string>();
    }
}
