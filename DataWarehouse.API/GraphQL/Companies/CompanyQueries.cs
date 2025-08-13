using DataWarehouse.API.Data;
using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataWarehouse.API.GraphQL.Companies;

public class CompanyQueries
{
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Company> GetCompanies([Service] IDbContextFactory<ApplicationDbContext> dbFactory)
        => dbFactory.CreateDbContext().Companies.AsNoTracking();
    
    public async Task<Company?> GetCompanyById(
        Guid id,
        [Service] IDbContextFactory<ApplicationDbContext> dbFactory,
        CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
    }
}