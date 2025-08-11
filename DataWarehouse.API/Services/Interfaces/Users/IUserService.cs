using DataWarehouse.Models.DTOs;
using DataWarehouse.Models.Interfaces;

namespace DataWarehouse.API.Services.Interfaces.Users
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetByIdAsync(Guid id);
        Task<UserProfileDto> RegisterAsync(RegisterUserDto dto, IUserIdentity? currentUser = null);
        Task<UserProfileDto>  UpdateAsync(Guid targetUserId, UpdateUserDto dto, IUserIdentity requester);
        Task                  DeleteAsync(Guid targetUserId, IUserIdentity requester);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);              
        Task<AuthResponseDto> RefreshAsync(string refreshToken);       
        Task LogoutAsync(string refreshToken);           
    }
}