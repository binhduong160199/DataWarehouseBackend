using DataWarehouse.API.Repositories.Interfaces.Companies;
using DataWarehouse.API.Services.Interfaces.Companies;
using DataWarehouse.Models.DTOs;
using DataWarehouse.Models.Entities;

namespace DataWarehouse.API.Services.Implementation.Companies
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repo;

        public CompanyService(ICompanyRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllAsync()
        {
            var companies = await _repo.GetAllAsync();
            return companies.Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                Website = c.Website,
                ContactEmail = c.ContactEmail,
                PhoneNumber = c.PhoneNumber,
                Industry = c.Industry,
                TaxId = c.TaxId,
                LogoUrl = c.LogoUrl,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<CompanyDto?> GetByIdAsync(Guid id)
        {
            var company = await _repo.GetByIdAsync(id);
            if (company == null) return null;

            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Address = company.Address,
                Website = company.Website,
                ContactEmail = company.ContactEmail,
                PhoneNumber = company.PhoneNumber,
                Industry = company.Industry,
                TaxId = company.TaxId,
                LogoUrl = company.LogoUrl,
                IsActive = company.IsActive,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt
            };
        }

        public async Task<CompanyDto> CreateAsync(CreateUpdateCompanyDto dto)
        {
            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Address = dto.Address,
                Website = dto.Website,
                ContactEmail = dto.ContactEmail,
                PhoneNumber = dto.PhoneNumber,
                Industry = dto.Industry,
                TaxId = dto.TaxId,
                LogoUrl = dto.LogoUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(company);

            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Address = company.Address,
                Website = company.Website,
                ContactEmail = company.ContactEmail,
                PhoneNumber = company.PhoneNumber,
                Industry = company.Industry,
                TaxId = company.TaxId,
                LogoUrl = company.LogoUrl,
                IsActive = company.IsActive,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt
            };
        }

        public async Task<CompanyDto?> UpdateAsync(Guid id, CreateUpdateCompanyDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Address = dto.Address;
            existing.Website = dto.Website;
            existing.ContactEmail = dto.ContactEmail;
            existing.PhoneNumber = dto.PhoneNumber;
            existing.Industry = dto.Industry;
            existing.TaxId = dto.TaxId;
            existing.LogoUrl = dto.LogoUrl;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);

            return new CompanyDto
            {
                Id = existing.Id,
                Name = existing.Name,
                Address = existing.Address,
                Website = existing.Website,
                ContactEmail = existing.ContactEmail,
                PhoneNumber = existing.PhoneNumber,
                Industry = existing.Industry,
                TaxId = existing.TaxId,
                LogoUrl = existing.LogoUrl,
                IsActive = existing.IsActive,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = existing.UpdatedAt
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var company = await _repo.GetByIdAsync(id);
            if (company == null) return false;

            await _repo.DeleteAsync(company);
            return true;
        }
    }
}