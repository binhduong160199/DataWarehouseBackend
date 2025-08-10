using DataWarehouse.API.Repositories.Interfaces.Users;
using DataWarehouse.API.Services.Interfaces.Users;
using DataWarehouse.API.Utils.Hash;
using DataWarehouse.Models.DTOs;
using DataWarehouse.Models.Entities;
using DataWarehouse.Models.Interfaces;
using DataWarehouse.API.Utils.Jwt;
using DataWarehouse.Models.Enums;
using StackExchange.Redis;

namespace DataWarehouse.API.Services.Implementation.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IHashUtility _hashUtility;
        private readonly IJwtUtility _jwt;
        
        public UserService(IUserRepository userRepo, IHashUtility hashUtility, IJwtUtility jwt)
        {
            _userRepo = userRepo;
            _hashUtility = hashUtility;
            _jwt = jwt;
        }

        public async Task<UserProfileDto> RegisterAsync(RegisterUserDto dto, IUserIdentity? currentUser)
        {
            var isCreatingAdmin = dto.Role == "Admin";
            var isCreatingUser = dto.Role == "User";

            if (isCreatingUser && (!dto.IsAdminCreating || currentUser?.Role != "Admin"))
                throw new Exception("Only an admin can create a normal user.");
            
            if (await _userRepo.UserExistsAsync(dto.Username))
                throw new Exception("Username already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = _hashUtility.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Birthday = dto.Birthday,
                CompanyId = dto.CompanyId,
                Role = Enum.TryParse<UserRole>(dto.Role, true, out var parsedRole) ? parsedRole : UserRole.User
            };

            await _userRepo.AddUserAsync(user);

            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Birthday = user.Birthday,
                CompanyId = user.CompanyId,
                CompanyName = user.Company?.Name ?? string.Empty
            };
        }
        
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByUsernameAsync(dto.Username);
            if (user == null)
                throw new Exception("Invalid username or password.");

            if (!_hashUtility.VerifyPassword(user.PasswordHash, dto.Password))
                throw new Exception("Invalid username or password.");

            var token = _jwt.GenerateJwtToken(user); // user implements IUserIdentity
            // Your JwtUtility uses 15 minutes; mirror that here
            var expiresAt = DateTime.UtcNow.AddMinutes(15);

            return new AuthResponseDto
            {
                AccessToken = token,
                ExpiresAtUtc = expiresAt,
                User = new UserProfileDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Birthday = user.Birthday,
                    CompanyId = user.CompanyId,
                    CompanyName = user.Company?.Name ?? string.Empty,
                    Role = user.Role.ToString()
                }
            };
        }
    }
}