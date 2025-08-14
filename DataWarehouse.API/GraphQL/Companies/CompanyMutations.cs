using DataWarehouse.API.Services.Interfaces.Companies;
using DataWarehouse.Models.DTOs;
using HotChocolate.Authorization;

namespace DataWarehouse.API.GraphQL.Companies;

[ExtendObjectType(typeof(Mutation))]
public class CompanyMutations
{
    [Authorize(Roles = new[] { "Owner" })]
    public async Task<CompanyDto> RegisterCompany(
        RegisterCompanyInput input,
        [Service] ICompanyService companyService)
    {
        var dto = new CreateUpdateCompanyDto
        {
            Name = input.Name,
            Address = input.Address,
            Website = input.Website,
            ContactEmail = input.ContactEmail,
            PhoneNumber = input.PhoneNumber,
            Industry = input.Industry,
            TaxId = input.TaxId,
            LogoUrl = input.LogoUrl
        };

        return await companyService.CreateAsync(dto);
    }

    [Authorize(Roles = new[] { "Owner" })]
    public async Task<CompanyDto?> UpdateCompany(
        UpdateCompanyInput input,
        [Service] ICompanyService companyService)
    {
        var dto = new CreateUpdateCompanyDto
        {
            Name = input.Name,
            Address = input.Address,
            Website = input.Website,
            ContactEmail = input.ContactEmail,
            PhoneNumber = input.PhoneNumber,
            Industry = input.Industry,
            TaxId = input.TaxId,
            LogoUrl = input.LogoUrl
        };

        return await companyService.UpdateAsync(input.Id, dto);
    }

    [Authorize(Roles = new[] { "Owner" })]
    public async Task<bool> DeleteCompany(
        Guid id,
        [Service] ICompanyService companyService)
    {
        return await companyService.DeleteAsync(id);
    }
}