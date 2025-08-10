using DataWarehouse.API.Data;
using DataWarehouse.API.Repositories.Interfaces.Companies;
using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataWarehouse.API.Repositories.Implementation.Companies
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
            => await _context.Companies.ToListAsync();

        public async Task<Company?> GetByIdAsync(Guid id)
            => await _context.Companies.FindAsync(id);

        public async Task AddAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Company company)
        {
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
        }
    }
}