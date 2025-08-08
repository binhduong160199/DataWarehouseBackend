using DataWarehouse.Models.DTOs;

namespace DataWarehouse.API.Services.Interfaces.Companies
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllAsync();
        Task<CompanyDto?> GetByIdAsync(Guid id);
        Task<CompanyDto> CreateAsync(CreateUpdateCompanyDto dto);
        Task<CompanyDto?> UpdateAsync(Guid id, CreateUpdateCompanyDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}