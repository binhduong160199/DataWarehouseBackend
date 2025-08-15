using System.Security.Claims;
using DataWarehouse.API.Services.Interfaces.Companies;
using DataWarehouse.API.Utils.Authorization;
using DataWarehouse.Models.DTOs;
using HotChocolate.Authorization;

namespace DataWarehouse.API.GraphQL.Companies;

[ExtendObjectType(typeof(Mutation))]
public class CompanyMutations
{
    [Authorize]
    public async Task<CompanyDto> RegisterCompany(
        RegisterCompanyInput input,
        ClaimsPrincipal principal,
        [Service] ICompanyService companyService)
    {
        var requester = principal.GetUserIdentity();
        if (requester is null)
            throw new GraphQLException("Unauthorized");

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
        return await companyService.CreateAsync(dto, requester);
    }

    [Authorize]
    public async Task<CompanyDto?> UpdateCompany(
        UpdateCompanyInput input,
        ClaimsPrincipal principal,
        [Service] ICompanyService companyService)
    {
        var requester = principal.GetUserIdentity();
        if (requester is null)
            throw new GraphQLException("Unauthorized");
        
        if (string.IsNullOrWhiteSpace(input.Name))
            throw new ValidationException("Company name is required.");
        
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

        return await companyService.UpdateAsync(input.Id, dto, requester);
    }

    [Authorize]
    public async Task<bool> DeleteCompany(
        Guid id,
        ClaimsPrincipal principal,
        [Service] ICompanyService companyService)
    {
        var requester = principal.GetUserIdentity();
        if (requester is null)
            throw new GraphQLException("Unauthorized");

        return await companyService.DeleteAsync(id, requester); 
    }
}