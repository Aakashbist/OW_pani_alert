
namespace HT.Overwatch.API.Common
{
    public static class Policies
    {
        public const string AdminUser = "AdminUser";
        public const string NormalUser = "NormalUser";
        public const string StandardUser = "StandardUser";
        public const string OutOfGroupUser = "OutOfGroupUser";

        public static readonly string[] All =
        {
             AdminUser,
             NormalUser,
             StandardUser,
             OutOfGroupUser
        };
    }
}
