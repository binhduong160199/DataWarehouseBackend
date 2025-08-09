using System.Security.Claims;
using DataWarehouse.Models.Interfaces;

namespace DataWarehouse.API.Utils.Authorization
{
    public static class ClaimsPrincipalExtensions
    {
        public static IUserIdentity? GetUserIdentity(this ClaimsPrincipal user)
        {
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
                return null;

            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(idClaim) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
                return null;

            return new SimpleUserIdentity(Guid.Parse(idClaim), username, role);
        }

        private class SimpleUserIdentity : IUserIdentity
        {
            public SimpleUserIdentity(Guid id, string username, string role)
            {
                Id = id;
                Username = username;
                Role = role;
            }

            public Guid Id { get; }
            public string Username { get; }
            public string Role { get; }
        }
    }
}