using DataWarehouse.API.Repositories.Interfaces.Users;
using DataWarehouse.API.Services.Interfaces.Users;
using DataWarehouse.API.Utils.Hash;
using DataWarehouse.Models.DTOs;
using DataWarehouse.Models.Entities;
using DataWarehouse.Models.Interfaces;
using DataWarehouse.Models.Enums;
using StackExchange.Redis;

namespace DataWarehouse.API.Services.Implementation.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IHashUtility _hashUtility;

        public UserService(IUserRepository userRepo, IHashUtility hashUtility)
        {
            _userRepo = userRepo;
            _hashUtility = hashUtility;
        }

        public async Task<UserProfileDto> RegisterAsync(RegisterUserDto dto, IUserIdentity? currentUser)
        {
            var isCreatingAdmin = dto.Role == "Admin";
            var isCreatingUser = dto.Role == "User";

            if (await _userRepo.UserExistsAsync(dto.Username))
                throw new Exception("Username already exists.");
            
            if (isCreatingUser && (!dto.IsAdminCreating || currentUser?.Role != "Admin"))
                throw new Exception("Only an admin can create a normal user.");

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
    }
}