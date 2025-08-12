using System.Security.Claims;
using DataWarehouse.Models.Interfaces;

namespace DataWarehouse.API.Utils.Authorization
{
    public static class ClaimsPrincipalExtensions
    {
        public static IUserIdentity? GetUserIdentity(this ClaimsPrincipal principal)
        {
            if (principal?.Identity?.IsAuthenticated != true) return null;

            var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var name = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
            var role = principal.FindFirstValue(ClaimTypes.Role) ?? "User";

            if (!Guid.TryParse(id, out var guid)) return null;
            return new Identity(guid, name, role);
        }

        private sealed record Identity(Guid Id, string Username, string Role) : IUserIdentity;
    }
}