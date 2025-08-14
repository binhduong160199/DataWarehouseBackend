using System.Security.Cryptography;
using DataWarehouse.API.Repositories.Interfaces.Users;
using DataWarehouse.API.Services.Interfaces.Users;
using DataWarehouse.API.Utils.Hash;
using DataWarehouse.API.Utils.Jwt;
using DataWarehouse.API.Utils.Redis;
using DataWarehouse.Models.DTOs;
using DataWarehouse.Models.Entities;
using DataWarehouse.Models.Enums;
using DataWarehouse.Models.Interfaces;

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

            if (newRole != UserRole.Owner && dto.CompanyId == null)
                throw new Exception("Only Owner can register without a Company.");

            if (currentUser is null)
            {
                if (newRole == UserRole.Owner)
                {
                    // allow anonymous Owner creation
                }
                else
                {
                    throw new Exception("Only Owner can register the first Admin.");
                }
            }
            else
            {
                var isRequesterOwner = string.Equals(currentUser.Role, "Owner", StringComparison.OrdinalIgnoreCase);
                var isRequesterAdmin = string.Equals(currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase);

                if (newRole == UserRole.Admin && !isRequesterOwner)
                    throw new Exception("Only Owner can create the first Admin.");

                if (newRole == UserRole.User && !isRequesterAdmin)
                    throw new Exception("Only Admin can create users.");
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
                JobTitle = dto.JobTitle,
                Department = dto.Department,
                ProfileImageUrl = dto.ProfileImageUrl,
                Role = newRole
            };

            await _users.AddUserAsync(user);
            return ToProfile(user);
        }

        public async Task<UserProfileDto> UpdateAsync(Guid targetUserId, UpdateUserDto dto, IUserIdentity requester)
        {
            var target = await _users.GetByIdAsync(targetUserId) ?? throw new Exception("User not found.");
            var isAdmin = string.Equals(requester.Role, "Admin", StringComparison.OrdinalIgnoreCase);
            var isSelf = requester.Id == targetUserId;

            if (!isAdmin && !isSelf) throw new Exception("Forbidden.");

            if (!string.IsNullOrWhiteSpace(dto.FirstName)) target.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.LastName)) target.LastName = dto.LastName;
            if (!string.IsNullOrWhiteSpace(dto.Email)) target.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) target.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(dto.JobTitle)) target.JobTitle = dto.JobTitle;
            if (!string.IsNullOrWhiteSpace(dto.Department)) target.Department = dto.Department;
            if (!string.IsNullOrWhiteSpace(dto.ProfileImageUrl)) target.ProfileImageUrl = dto.ProfileImageUrl;
            if (dto.Birthday.HasValue) target.Birthday = dto.Birthday;

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                if (!isSelf && !isAdmin) throw new Exception("Forbidden.");
                target.PasswordHash = _hash.HashPassword(dto.NewPassword);
            }

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                if (!isAdmin)
                    throw new Exception("Only admin can change roles.");

                if (!Enum.TryParse<UserRole>(dto.Role, true, out var newRole))
                    throw new Exception("Invalid role.");

                if (target.Role == UserRole.Admin && newRole != UserRole.Admin)
                {
                    var adminCount = target.CompanyId.HasValue
                        ? await _users.CountCompanyAdminsAsync(target.CompanyId.Value)
                        : 0;

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
                var adminCount = target.CompanyId.HasValue
                    ? await _users.CountCompanyAdminsAsync(target.CompanyId.Value)
                    : 0;

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

            var now = DateTime.UtcNow;
            await _users.UpdateLastLoginAsync(user.Id, now);
            user.LastLoginAt = now; 
            
            var accessToken = _jwt.GenerateJwtToken(user);
            var refreshToken = GenerateOpaqueToken();

            var key = RefreshKey(refreshToken);
            await _redis.SetCacheAsync(key, new RefreshRecord { Username = user.Username }, RefreshTtl);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                ExpiresAtUtc = DateTime.UtcNow.Add(AccessTtl),
                RefreshToken = refreshToken,
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
            
            if (!string.IsNullOrWhiteSpace(key))
                await _redis.DeleteCacheAsync(key);
            var newRefresh = GenerateOpaqueToken();
            await _redis.SetCacheAsync(RefreshKey(newRefresh), new RefreshRecord { Username = record.Username }, RefreshTtl);

            var user = await _users.GetByUsernameAsync(record.Username) ?? throw new Exception("User not found.");
            var access = _jwt.GenerateJwtToken(user);

            return new AuthResponseDto
            {
                AccessToken = access,
                ExpiresAtUtc = DateTime.UtcNow.Add(AccessTtl),
                RefreshToken = newRefresh,
                User = ToProfile(user)
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            var key = RefreshKey(refreshToken!);
            await _redis.DeleteCacheAsync(key);
        }

        private static string RefreshKey(string refreshToken)
        {
            ArgumentNullException.ThrowIfNull(refreshToken);
            return $"refresh:{refreshToken}";
        }

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
            JobTitle = u.JobTitle,
            Department = u.Department,
            ProfileImageUrl = u.ProfileImageUrl,
            LastLoginAt = u.LastLoginAt,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
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