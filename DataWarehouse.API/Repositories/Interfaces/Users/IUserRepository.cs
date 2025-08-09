using DataWarehouse.Models.Entities;

namespace DataWarehouse.API.Repositories.Interfaces.Users
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task<bool> UserExistsAsync(string username);
    }
}