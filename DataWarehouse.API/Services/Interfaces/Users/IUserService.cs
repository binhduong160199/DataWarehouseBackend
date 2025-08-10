using DataWarehouse.Models.DTOs;
using DataWarehouse.Models.Interfaces;

namespace DataWarehouse.API.Services.Interfaces.Users
{
    public interface IUserService
    {
        Task<UserProfileDto> RegisterAsync(RegisterUserDto dto, IUserIdentity? currentUser = null);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}