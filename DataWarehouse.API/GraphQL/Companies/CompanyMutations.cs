using DataWarehouse.API.Data;
using DataWarehouse.Models.Entities;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DataWarehouse.API.GraphQL.Companies;

[ExtendObjectType(typeof(Mutation))] 
public class CompanyMutations
{
    [Authorize(Roles = new[] { "Owner" })] 
    public async Task<Company> RegisterCompany(
        RegisterCompanyInput input,
        [Service] IDbContextFactory<ApplicationDbContext> dbFactory,
        CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        
        var nameExists = await db.Companies.AnyAsync(c => c.Name == input.Name, ct);
        if (nameExists)
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage($"Company with name '{input.Name}' already exists.")
                .SetCode("COMPANY_NAME_EXISTS")
                .Build());
        }

        var entity = new Company
        {
            Id = Guid.NewGuid(),
            Name = input.Name,
            Address = input.Address,
            Website = input.Website,
            ContactEmail = input.ContactEmail,
            PhoneNumber = input.PhoneNumber,
            Industry = input.Industry,
            TaxId = input.TaxId,
            LogoUrl = input.LogoUrl,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        db.Companies.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    [Authorize(Roles = new[] { "Owner" })]
    public async Task<Company> UpdateCompany(
        UpdateCompanyInput input,
        [Service] IDbContextFactory<ApplicationDbContext> dbFactory,
        CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var entity = await db.Companies.FirstOrDefaultAsync(c => c.Id == input.Id, ct);
        if (entity is null)
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage($"Company with ID '{input.Id}' not found.")
                .SetCode("COMPANY_NOT_FOUND")
                .Build());
        }

        if (input.ContactEmail is not null) entity.ContactEmail = input.ContactEmail;
        if (input.PhoneNumber is not null) entity.PhoneNumber = input.PhoneNumber;
        if (input.Industry is not null) entity.Industry = input.Industry;
        if (input.TaxId is not null) entity.TaxId = input.TaxId;
        if (input.LogoUrl is not null) entity.LogoUrl = input.LogoUrl;
        if (input.IsActive.HasValue) entity.IsActive = input.IsActive.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return entity;
    }

    [Authorize(Roles = new[] { "Owner" })]
    public async Task<bool> DeleteCompany(
        Guid id,
        [Service] IDbContextFactory<ApplicationDbContext> dbFactory,
        CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.Companies.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null) return false;

        db.Companies.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}