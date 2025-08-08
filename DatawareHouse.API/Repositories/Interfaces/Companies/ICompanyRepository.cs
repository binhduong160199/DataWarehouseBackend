using DataWarehouse.Models.Entities;

namespace DataWarehouse.API.Repositories.Interfaces.Companies
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllAsync();
        Task<Company?> GetByIdAsync(Guid id);
        Task AddAsync(Company company);
        Task UpdateAsync(Company company);
        Task DeleteAsync(Company company);
    }
}