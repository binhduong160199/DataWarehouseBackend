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

        public async Task<UserProfileDto?> GetByIdAsync(Guid id)
        {
            var user = await _users.GetByIdAsync(id);
            return user == null ? null : ToProfile(user);
        }
        
        public async Task<UserProfileDto> RegisterAsync(RegisterUserDto dto, IUserIdentity? currentUser)
        {
            if (await _users.UserExistsAsync(dto.Username))
                throw new Exception("Username already exists.");
            
            var newRole = Enum.TryParse<UserRole>(dto.Role, true, out var parsedRole) ? parsedRole : UserRole.User;

            var companyAdminCount = await _users.CountCompanyAdminsAsync(dto.CompanyId);

            if (currentUser is null)
            {
                // Only allowed if company has ZERO admins and the new user is Admin
                if (companyAdminCount > 0)
                    throw new Exception("An admin already exists for this company. Authentication required.");

                if (newRole != UserRole.Admin)
                    throw new Exception("The first account for a company must be an Admin.");
                // allow creating first Admin without a token
            }
            else
            {
                var isRequesterAdmin = string.Equals(currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase);
                if (!isRequesterAdmin)
                    throw new Exception("Only admin can create users.");
            }

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
                Role = newRole
            };

            await _users.AddUserAsync(user);
            return ToProfile(user);
        }
        
        public async Task<UserProfileDto> UpdateAsync(Guid targetUserId, UpdateUserDto dto, IUserIdentity requester)
        {
            var target = await _users.GetByIdAsync(targetUserId) ?? throw new Exception("User not found.");
            var isAdmin = string.Equals(requester.Role, "Admin", StringComparison.OrdinalIgnoreCase);
            var isSelf  = requester.Id == targetUserId;

            if (!isAdmin && !isSelf) throw new Exception("Forbidden.");

            // basic fields (both admin & self can change these)
            if (!string.IsNullOrWhiteSpace(dto.FirstName))   target.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.LastName))    target.LastName  = dto.LastName;
            if (!string.IsNullOrWhiteSpace(dto.Email))       target.Email     = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) target.PhoneNumber = dto.PhoneNumber;
            if (dto.Birthday.HasValue)                       target.Birthday  = dto.Birthday;

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                if (!isSelf && !isAdmin) throw new Exception("Forbidden.");
                target.PasswordHash = _hash.HashPassword(dto.NewPassword);
            }

            // Role changes:
            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                if (!isAdmin)
                    throw new Exception("Only admin can change roles.");

                if (!Enum.TryParse<UserRole>(dto.Role, true, out var newRole))
                    throw new Exception("Invalid role.");

                // if demoting an Admin -> User, ensure not the last admin
                if (target.Role == UserRole.Admin && newRole != UserRole.Admin)
                {
                    var adminCount = await _users.CountCompanyAdminsAsync(target.CompanyId);
                    if (adminCount <= 1)
                        throw new Exception("Operation blocked: at least one admin must remain in the company.");
                }

                target.Role = newRole;
            }

            await _users.UpdateUserAsync(target);
            return ToProfile(target);
        }

        public async Task DeleteAsync(Guid targetUserId, IUserIdentity requester)
        {
            var target = await _users.GetByIdAsync(targetUserId);
            if (target == null) return;

            var isAdmin = string.Equals(requester.Role, "Admin", StringComparison.OrdinalIgnoreCase);
            if (!isAdmin) throw new Exception("Forbidden.");

            if (target.Role == UserRole.Admin)
            {
                var adminCount = await _users.CountCompanyAdminsAsync(target.CompanyId);
                if (adminCount <= 1)
                    throw new Exception("Operation blocked: cannot delete the last admin in the company.");
            }

            await _users.DeleteUserAsync(target);
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