using DataWarehouse.Models.DTOs;
using DataWarehouse.Models.Interfaces;

namespace DataWarehouse.API.Services.Interfaces.Companies;

public interface ICompanyService
{
    Task<IEnumerable<CompanyDto>> GetAllAsync();
    Task<CompanyDto?> GetByIdAsync(Guid id);
    Task<CompanyDto> CreateAsync(CreateCompanyDto dto, IUserIdentity requester); 
    Task<CompanyDto?> UpdateAsync(Guid id, UpdateCompanyDto dto, IUserIdentity requester); 
    Task<bool> DeleteAsync(Guid id, IUserIdentity requester); 
}