using System.Security.Cryptography;
using DataWarehouse.API.Repositories.Interfaces.Users;
using DataWarehouse.API.Services.Interfaces.Users;
using DataWarehouse.API.Utils.Hash;
using DataWarehouse.API.Utils.Jwt;
using DataWarehouse.API.Utils.Redis;
using DataWarehouse.API.Utils.Mapping;
using DataWarehouse.API.Utils.Authorization;
using DataWarehouse.API.Utils.Exceptions;
using DataWarehouse.Models.DTOs;
using DataWarehouse.Models.Entities;
using DataWarehouse.Models.Enums;
using DataWarehouse.Models.Interfaces;
using FluentValidation;

namespace DataWarehouse.API.Services.Implementation.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IHashUtility _hash;
        private readonly IJwtUtility _jwt;
        private readonly RedisHelper _redis;
        private readonly ILogger<UserService> _logger;
        private readonly IValidator<RegisterUserDto> _registerValidator;
        private readonly IValidator<UpdateUserDto> _updateValidator;
        private static readonly TimeSpan AccessTtl = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan RefreshTtl = TimeSpan.FromDays(7);
        private static readonly TimeSpan UserCacheTtl = TimeSpan.FromHours(6);

        public UserService(
            IUserRepository users,
            IHashUtility hash,
            IJwtUtility jwt,
            RedisHelper redis,
            ILogger<UserService> logger,
            IValidator<RegisterUserDto> registerValidator,
            IValidator<UpdateUserDto> updateValidator)
        {
            _users = users;
            _hash = hash;
            _jwt = jwt;
            _redis = redis;
            _logger = logger;
            _registerValidator = registerValidator;
            _updateValidator = updateValidator;
        }

        public async Task<UserProfileDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"user:id:{id}";
            var cached = await _redis.GetCacheAsync<UserProfileDto>(cacheKey);
            if (cached != null) return cached;

            var user = await _users.GetByIdAsync(id);
            if (user == null) return null;

            var profile = UserMapping.ToProfile(user);
            await _redis.SetCacheAsync(cacheKey, profile, UserCacheTtl);
            return profile;
        }

        public async Task<UserProfileDto> RegisterAsync(RegisterUserDto dto, IUserIdentity? currentUser)
        {
            await _registerValidator.ValidateAndThrowAsync(dto);
            
            if (await _users.UserExistsAsync(dto.Username))
                throw new ValidationException("Username already exists.");

            var role = ParseRoleOrDefault(dto.Role);
            await ValidateRegistrationPermissionsAsync(role, dto.CompanyId, currentUser);

            _logger.LogInformation("Registering new user {Username} with role {Role}", dto.Username, role);

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
                Role = role
            };

            await _users.AddUserAsync(user);
            var profile = UserMapping.ToProfile(user);
            await _redis.SetCacheAsync($"user:id:{user.Id}", profile, UserCacheTtl);

            _logger.LogInformation("User {UserId} registered successfully", user.Id);
            return profile;
        }

        private static UserRole ParseRoleOrDefault(string? roleString)
        {
            return Enum.TryParse<UserRole>(roleString, true, out var parsedRole) ? parsedRole : UserRole.User;
        }

        private async Task ValidateRegistrationPermissionsAsync(UserRole role, Guid? companyId, IUserIdentity? currentUser)
        {
            if (role != UserRole.Owner && companyId == null)
                throw new ValidationException("Only Owner can register without a Company.");

            if (currentUser is null)
            {
                if (role != UserRole.Owner)
                    throw new ForbiddenException("Only Owner can register the first Admin.");
                return;
            }

            var isOwner = RoleCheck.IsOwner(currentUser);
            var isAdmin = RoleCheck.IsAdmin(currentUser);

            if (role == UserRole.Admin)
            {
                var adminExists = companyId.HasValue && await _users.CountCompanyAdminsAsync(companyId.Value) > 0;
                if (!adminExists && !isOwner)
                    throw new ForbiddenException("Only Owner can create the first Admin.");
            }

            if (role == UserRole.User && !isAdmin)
                throw new ForbiddenException("Only Admin can create users.");
        }

        public async Task<UserProfileDto> UpdateAsync(Guid targetUserId, UpdateUserDto dto, IUserIdentity requester)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);
            
            var target = await _users.GetByIdAsync(targetUserId) ?? throw new NotFoundException("User not found.");
            var isAdmin = RoleCheck.IsAdmin(requester);
            var isSelf = requester.Id == targetUserId;

            if (!isAdmin && !isSelf)
                throw new ForbiddenException("You are not allowed to update this user.");

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
                if (!isSelf && !isAdmin)
                    throw new ForbiddenException("You are not allowed to update this password.");
                target.PasswordHash = _hash.HashPassword(dto.NewPassword);
            }

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                if (!isAdmin)
                    throw new ForbiddenException("Only admin can change roles.");

                if (!Enum.TryParse<UserRole>(dto.Role, true, out var newRole))
                    throw new ValidationException("Invalid role.");

                if (target.Role == UserRole.Admin && newRole != UserRole.Admin)
                {
                    var adminCount = target.CompanyId.HasValue
                        ? await _users.CountCompanyAdminsAsync(target.CompanyId.Value)
                        : 0;

                    if (adminCount <= 1)
                        throw new ValidationException("At least one admin must remain in the company.");
                }

                target.Role = newRole;
            }

            await _users.UpdateUserAsync(target);
            await _redis.DeleteCacheAsync($"user:id:{targetUserId}");
            return UserMapping.ToProfile(target);
        }

        public async Task DeleteAsync(Guid targetUserId, IUserIdentity requester)
        {
            var target = await _users.GetByIdAsync(targetUserId);
            if (target == null) throw new NotFoundException("User not found.");

            var isAdmin = RoleCheck.IsAdmin(requester);
            if (!isAdmin) throw new ForbiddenException("You are not allowed to delete this user.");

            if (target.Role == UserRole.Admin)
            {
                var adminCount = target.CompanyId.HasValue
                    ? await _users.CountCompanyAdminsAsync(target.CompanyId.Value)
                    : 0;

                if (adminCount <= 1)
                    throw new ValidationException("Cannot delete the last admin in the company.");
            }

            _logger.LogWarning("User {RequesterId} is deleting user {TargetUserId}", requester.Id, targetUserId);
            await _users.DeleteUserAsync(target);
            await _redis.DeleteCacheAsync($"user:id:{targetUserId}");
            _logger.LogInformation("User {TargetUserId} deleted successfully", targetUserId);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Attempting login for user {Username}", dto.Username);

            var user = await _users.GetByUsernameAsync(dto.Username);
            if (user == null || !_hash.VerifyPassword(user.PasswordHash, dto.Password))
            {
                _logger.LogWarning("Login failed for user {Username}", dto.Username);
                throw new UnauthorizedException("Invalid username or password.");
            }

            var now = DateTime.UtcNow;
            await _users.UpdateLastLoginAsync(user.Id, now);
            user.LastLoginAt = now;

            await _redis.DeleteCacheAsync($"user:id:{user.Id}");

            var accessToken = _jwt.GenerateJwtToken(user);
            var refreshToken = GenerateOpaqueToken();

            var key = RefreshKey(refreshToken);
            await _redis.SetCacheAsync(key, new RefreshRecord { Username = user.Username }, RefreshTtl);

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                ExpiresAtUtc = DateTime.UtcNow.Add(AccessTtl),
                RefreshToken = refreshToken,
                User = UserMapping.ToProfile(user)
            };
        }

        public async Task<AuthResponseDto> RefreshAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ValidationException("Missing refresh token.");

            var key = RefreshKey(refreshToken);
            var record = await _redis.GetCacheAsync<RefreshRecord>(key);
            if (record == null)
                throw new UnauthorizedException("Invalid or expired refresh token.");

            await _redis.DeleteCacheAsync(key);
            var newRefresh = GenerateOpaqueToken();
            await _redis.SetCacheAsync(RefreshKey(newRefresh), new RefreshRecord { Username = record.Username }, RefreshTtl);

            var user = await _users.GetByUsernameAsync(record.Username)
                       ?? throw new NotFoundException("User not found.");

            var access = _jwt.GenerateJwtToken(user);

            return new AuthResponseDto
            {
                AccessToken = access,
                ExpiresAtUtc = DateTime.UtcNow.Add(AccessTtl),
                RefreshToken = newRefresh,
                User = UserMapping.ToProfile(user)
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            var key = RefreshKey(refreshToken);
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

        private class RefreshRecord
        {
            public string Username { get; set; } = string.Empty;
        }
    }
}
