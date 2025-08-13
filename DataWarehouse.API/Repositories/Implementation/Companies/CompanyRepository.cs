using DataWarehouse.API.Data;
using DataWarehouse.API.Repositories.Interfaces.Companies;
using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataWarehouse.API.Repositories.Implementation.Companies
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public CompanyRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Companies.ToListAsync();
        }

        public async Task<Company?> GetByIdAsync(Guid id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Companies.FindAsync(id);
        }

        public async Task AddAsync(Company company)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Companies.Add(company);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Company company)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Companies.Update(company);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Company company)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Companies.Remove(company);
            await context.SaveChangesAsync();
        }
    }
}