using DataWarehouse.Models.Interfaces;

namespace DataWarehouse.API.Utils.Authorization
{
    public static class RoleCheck
    {
        public static bool IsAdmin(IUserIdentity? user) =>
            user != null && string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase);

        public static bool IsOwner(IUserIdentity? user) =>
            user != null && string.Equals(user.Role, "Owner", StringComparison.OrdinalIgnoreCase);

        public static bool IsUser(IUserIdentity? user) =>
            user != null && string.Equals(user.Role, "User", StringComparison.OrdinalIgnoreCase);
    }
}