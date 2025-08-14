using DataWarehouse.API.Data;
using DataWarehouse.API.Repositories.Interfaces.Users;
using DataWarehouse.Models.Entities;
using DataWarehouse.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataWarehouse.API.Repositories.Implementation.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public UserRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            return await db.Users.Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            return await db.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddUserAsync(User user)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            db.Users.Update(user);
            await db.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            db.Users.Remove(user);
            await db.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            return await db.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<int> CountCompanyAdminsAsync(Guid companyId)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            return await db.Users
                .Where(u => u.CompanyId == companyId && u.Role == UserRole.Admin)
                .CountAsync();
        }
        
        public async Task UpdateLastLoginAsync(Guid userId, DateTime lastLogin)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            var user = new User { Id = userId };
            db.Users.Attach(user);
            db.Entry(user).Property(u => u.LastLoginAt).CurrentValue = lastLogin;
            db.Entry(user).Property(u => u.LastLoginAt).IsModified = true;
            await db.SaveChangesAsync();
        }
    }
}