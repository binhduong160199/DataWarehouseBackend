using DataWarehouse.Models.Entities;

namespace DataWarehouse.API.Repositories.Interfaces.Users
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task<bool> UserExistsAsync(string username);
        Task<int> CountCompanyAdminsAsync(Guid companyId);
        Task UpdateLastLoginAsync(Guid userId, DateTime lastLogin);
    }
}