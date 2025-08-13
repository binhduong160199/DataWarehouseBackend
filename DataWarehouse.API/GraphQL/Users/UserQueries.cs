using System.Security.Claims;
using HotChocolate.Authorization;
using DataWarehouse.API.Services.Interfaces.Users;
using DataWarehouse.Models.DTOs;

namespace DataWarehouse.API.GraphQL.Users
{
    [ExtendObjectType(typeof(DataWarehouse.API.GraphQL.Query))]
    public class UserQueries
    {
        [Authorize]
        public async Task<UserProfileDto?> Me(
            [Service] IUserService users,
            ClaimsPrincipal principal)
        {
            var idValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(idValue, out var id)) return null;
            return await users.GetByIdAsync(id);
        }

        [Authorize(Roles = new[] { "Admin" })]
        public Task<UserProfileDto?> UserById(
            Guid id,
            [Service] IUserService users)
            => users.GetByIdAsync(id);
    }
}