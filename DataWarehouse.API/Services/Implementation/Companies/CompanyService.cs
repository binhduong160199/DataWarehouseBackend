using DataWarehouse.API.Repositories.Interfaces.Companies;
using DataWarehouse.API.Services.Interfaces.Companies;
using DataWarehouse.API.Utils.Authorization;
using DataWarehouse.API.Utils.Mapping;
using DataWarehouse.API.Utils.Redis;
using DataWarehouse.Models.DTOs;
using DataWarehouse.Models.Entities;
using DataWarehouse.Models.Interfaces;
using DataWarehouse.API.Utils.Exceptions;
using FluentValidation;

namespace DataWarehouse.API.Services.Implementation.Companies
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repo;
        private readonly RedisHelper _redis;
        private readonly ILogger<CompanyService> _logger;
        private readonly IValidator<CreateCompanyDto> _createValidator;
        private readonly IValidator<UpdateCompanyDto> _updateValidator;
        private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(6);

        public CompanyService(
            ICompanyRepository repo,
            RedisHelper redis,
            ILogger<CompanyService> logger,
            IValidator<CreateCompanyDto> createValidator,
            IValidator<UpdateCompanyDto> updateValidator)
        {
            _repo = repo;
            _redis = redis;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllAsync()
        {
            var cacheKey = "company:all";
            var cached = await _redis.GetCacheAsync<IEnumerable<CompanyDto>>(cacheKey);
            if (cached != null) return cached;

            var companies = await _repo.GetAllAsync();
            var result = companies.Select(CompanyMapping.ToDto).ToList();

            await _redis.SetCacheAsync(cacheKey, result, CacheTtl);
            return result;
        }

        public async Task<CompanyDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"company:id:{id}";
            var cached = await _redis.GetCacheAsync<CompanyDto>(cacheKey);
            if (cached != null) return cached;

            var company = await _repo.GetByIdAsync(id);
            if (company == null) return null;

            var dto = CompanyMapping.ToDto(company);
            await _redis.SetCacheAsync(cacheKey, dto, CacheTtl);
            return dto;
        }

        public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto, IUserIdentity requester)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            
            if (!RoleCheck.IsOwner(requester))
                throw new ForbiddenException("Only Owner can create a company.");

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

            _logger.LogInformation("Creating company {Name}", dto.Name);

            await _repo.AddAsync(company);
            await _redis.DeleteCacheAsync("company:all");

            return CompanyMapping.ToDto(company);
        }

        public async Task<CompanyDto?> UpdateAsync(Guid id, UpdateCompanyDto dto, IUserIdentity requester)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);
            
            if (!RoleCheck.IsAdmin(requester) && !RoleCheck.IsOwner(requester))
                throw new ForbiddenException("Only Admin or Owner can update a company.");

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException($"Company {id} not found.");

            if (dto.Name != null) existing.Name = dto.Name;
            if (dto.Address != null) existing.Address = dto.Address;
            if (dto.Website != null) existing.Website = dto.Website;
            if (dto.ContactEmail != null) existing.ContactEmail = dto.ContactEmail;
            if (dto.PhoneNumber != null) existing.PhoneNumber = dto.PhoneNumber;
            if (dto.Industry != null) existing.Industry = dto.Industry;
            if (dto.TaxId != null) existing.TaxId = dto.TaxId;
            if (dto.LogoUrl != null) existing.LogoUrl = dto.LogoUrl;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);

            var cacheKey = $"company:id:{id}";
            await _redis.DeleteCacheAsync(cacheKey);
            await _redis.DeleteCacheAsync("company:all");

            _logger.LogInformation("Updated company {Id}", id);

            return CompanyMapping.ToDto(existing);
        }

        public async Task<bool> DeleteAsync(Guid id, IUserIdentity requester)
        {
            if (!RoleCheck.IsOwner(requester))
                throw new ForbiddenException("Only Owner can delete a company.");

            var company = await _repo.GetByIdAsync(id);
            if (company == null)
                throw new NotFoundException($"Company {id} not found.");

            await _repo.DeleteAsync(company);

            var cacheKey = $"company:id:{id}";
            await _redis.DeleteCacheAsync(cacheKey);
            await _redis.DeleteCacheAsync("company:all");

            _logger.LogInformation("Deleted company {Id}", id);

            return true;
        }
    }
}