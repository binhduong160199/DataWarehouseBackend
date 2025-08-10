using DataWarehouse.Models.Interfaces;
using System.Security.Claims;

namespace DataWarehouse.API.Utils.Jwt
{
    public interface IJwtUtility
    {
        string GenerateJwtToken(IUserIdentity user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}