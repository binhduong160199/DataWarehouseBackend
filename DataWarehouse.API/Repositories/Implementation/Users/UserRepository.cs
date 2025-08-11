using DataWarehouse.API.Data;
using DataWarehouse.API.Repositories.Interfaces.Users;
using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataWarehouse.API.Repositories.Implementation.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.Include(u => u.Company).FirstOrDefaultAsync(u => u.Username == username);
        }
        
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}