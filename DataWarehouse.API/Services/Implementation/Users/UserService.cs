// UserService.cs
using System.Security.Cryptography;
using DataWarehouse.API.Repositories.Interfaces.Users;
using DataWarehouse.API.Services.Interfaces.Users;
using DataWarehouse.API.Utils.Hash;
using DataWarehouse.API.Utils.Jwt;
using DataWarehouse.Models.DTOs;
using DataWarehouse.Models.Entities;
using DataWarehouse.Models.Enums;
using DataWarehouse.Models.Interfaces;
using DataWarehouse.Utils.Redis;

namespace DataWarehouse.API.Services.Implementation.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IHashUtility _hash;
        private readonly IJwtUtility _jwt;
        private readonly RedisHelper _redis;

        private static readonly TimeSpan AccessTtl = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan RefreshTtl = TimeSpan.FromDays(7);

        public UserService(IUserRepository users, IHashUtility hash, IJwtUtility jwt, RedisHelper redis)
        {
            _users = users;
            _hash = hash;
            _jwt = jwt;
            _redis = redis;
        }

        public async Task<UserProfileDto> RegisterAsync(RegisterUserDto dto, IUserIdentity? currentUser)
        {
            var isCreatingUser = dto.Role == "User";
            if (isCreatingUser && (!dto.IsAdminCreating || currentUser?.Role != "Admin"))
                throw new Exception("Only an admin can create a normal user.");

            if (await _users.UserExistsAsync(dto.Username))
                throw new Exception("Username already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = _hash.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Birthday = dto.Birthday,
                CompanyId = dto.CompanyId,
                Role = Enum.TryParse<UserRole>(dto.Role, true, out var parsedRole) ? parsedRole : UserRole.User
            };

            await _users.AddUserAsync(user);
            return ToProfile(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _users.GetByUsernameAsync(dto.Username);
            if (user == null || !_hash.VerifyPassword(user.PasswordHash, dto.Password))
                throw new Exception("Invalid username or password.");

            var accessToken = _jwt.GenerateJwtToken(user);
            var refreshToken = GenerateOpaqueToken();

            // Save refresh token -> user mapping in Redis (value includes username so we can re-hydrate without new repo method)
            var key = RefreshKey(refreshToken);
            await _redis.SetCacheAsync(key, new RefreshRecord { Username = user.Username }, RefreshTtl);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                ExpiresAtUtc = DateTime.UtcNow.Add(AccessTtl),
                RefreshToken = refreshToken, // controller should set this as HttpOnly cookie, not return in body
                User = ToProfile(user)
            };
        }

        public async Task<AuthResponseDto> RefreshAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new Exception("Missing refresh token.");

            var key = RefreshKey(refreshToken);
            var record = await _redis.GetCacheAsync<RefreshRecord>(key);
            if (record == null)
                throw new Exception("Invalid or expired refresh token.");

            // rotate: delete old, issue new
            await _redis.DeleteCacheAsync(key);
            var newRefresh = GenerateOpaqueToken();
            await _redis.SetCacheAsync(RefreshKey(newRefresh), new RefreshRecord { Username = record.Username }, RefreshTtl);

            // load user (by username we stored) and issue new access
            var user = await _users.GetByUsernameAsync(record.Username) ?? throw new Exception("User not found.");
            var access = _jwt.GenerateJwtToken(user);

            return new AuthResponseDto
            {
                AccessToken = access,
                ExpiresAtUtc = DateTime.UtcNow.Add(AccessTtl),
                RefreshToken = newRefresh, // controller re-sets cookie
                User = ToProfile(user)
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) return;
            await _redis.DeleteCacheAsync(RefreshKey(refreshToken));
        }

        // ----- helpers -----

        private static string RefreshKey(string refreshToken) => $"refresh:{refreshToken}";

        private static string GenerateOpaqueToken()
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static UserProfileDto ToProfile(User u) => new()
        {
            Id = u.Id,
            Username = u.Username,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            Birthday = u.Birthday,
            CompanyId = u.CompanyId,
            CompanyName = u.Company?.Name ?? string.Empty,
            Role = u.Role.ToString()
        };

        private class RefreshRecord
        {
            public string Username { get; set; } = string.Empty;
        }
    }
}