using System.Security.Claims;
using DataWarehouse.API.Services.Interfaces.Users;
using DataWarehouse.Models.DTOs;
using DataWarehouse.API.Utils.Authorization;
using HotChocolate.Authorization;

namespace DataWarehouse.API.GraphQL.Users
{
    [ExtendObjectType(typeof(DataWarehouse.API.GraphQL.Mutation))]
    public class UserMutations
    {
        private const string RefreshCookieName = "refresh_token";
        private static readonly TimeSpan RefreshTtl = TimeSpan.FromDays(7);

        [Authorize]
        public async Task<UserProfileDto> Register(
            RegisterUserInput input,
            [Service] IUserService users,
            ClaimsPrincipal principal)
        {
            var dto = new RegisterUserDto
            {
                Username = input.Username,
                Password = input.Password,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                PhoneNumber = input.PhoneNumber,
                Birthday = input.Birthday,
                JobTitle = input.JobTitle,
                Department = input.Department,
                ProfileImageUrl = input.ProfileImageUrl,
                CompanyId = input.CompanyId,
                Role = input.Role,
            };
            var requester = principal.GetUserIdentity();
            if (requester == null)
                throw new GraphQLException("Unauthorized");
            return await users.RegisterAsync(dto, requester);
        }

        [Authorize]
        public async Task<UserProfileDto> UpdateUser(
            Guid userId,
            UpdateUserInput input,
            [Service] IUserService users,
            ClaimsPrincipal principal)
        {
            var dto = new UpdateUserDto
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                PhoneNumber = input.PhoneNumber,
                Birthday = input.Birthday,
                JobTitle = input.JobTitle,
                Department = input.Department,
                ProfileImageUrl = input.ProfileImageUrl,
                Role = input.Role,
                NewPassword = input.NewPassword
            };

            var requester = principal.GetUserIdentity();
            if (requester == null)
                throw new GraphQLException("Unauthorized");
            return await users.UpdateAsync(userId, dto, requester);
        }
        
        [Authorize]
        public async Task<bool> DeleteUser(
            Guid userId,
            [Service] IUserService users,
            ClaimsPrincipal principal)
        {
            var requester = principal.GetUserIdentity();
            if (requester == null)
                throw new GraphQLException("Unauthorized");
            await users.DeleteAsync(userId, requester);
            return true;
        }
        
        public async Task<AuthResponseDto> Login(
            LoginInput input,
            [Service] IUserService users,
            [Service] IHttpContextAccessor accessor)
        {
            var auth = await users.LoginAsync(new LoginDto
            {
                Username = input.Username,
                Password = input.Password
            });

            if (!string.IsNullOrWhiteSpace(auth.RefreshToken))
            {
                var env = accessor.HttpContext!.RequestServices.GetRequiredService<IWebHostEnvironment>();
                var cookie = CookieOptionsFactory.CreateRefreshCookie(env, RefreshTtl);
                accessor.HttpContext!.Response.Cookies.Append(RefreshCookieName, auth.RefreshToken!, cookie);
                auth.RefreshToken = null;
            }

            return auth;
        }

        public async Task<AuthResponseDto> Refresh(
            [Service] IUserService users,
            [Service] IHttpContextAccessor accessor)
        {
            var refresh = accessor.HttpContext!.Request.Cookies[RefreshCookieName] ?? string.Empty;
            var auth = await users.RefreshAsync(refresh);

            if (!string.IsNullOrWhiteSpace(auth.RefreshToken))
            {
                var env = accessor.HttpContext!.RequestServices.GetRequiredService<IWebHostEnvironment>();
                var cookie = CookieOptionsFactory.CreateRefreshCookie(env, RefreshTtl);
                accessor.HttpContext!.Response.Cookies.Append(RefreshCookieName, auth.RefreshToken!, cookie);
                auth.RefreshToken = null;
            }

            return auth;
        }

        public async Task<bool> Logout(
            [Service] IUserService users,
            [Service] IHttpContextAccessor accessor)
        {
            var refresh = accessor.HttpContext!.Request.Cookies[RefreshCookieName];

            if (!string.IsNullOrWhiteSpace(refresh))
            {
                await users.LogoutAsync(refresh);
                var env = accessor.HttpContext!.RequestServices.GetRequiredService<IWebHostEnvironment>();
                var del = CookieOptionsFactory.CreateRefreshCookie(env, RefreshTtl, expireNow: true);
                accessor.HttpContext!.Response.Cookies.Delete(RefreshCookieName, del);
            }

            return true;
        }
    }
}

