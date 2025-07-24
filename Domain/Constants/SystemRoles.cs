namespace Domain.Constants
{
    public class SystemRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Moderator = "Moderator";

        public static readonly string[] DefaultRoles = { Admin, User, Moderator };
    }
}
