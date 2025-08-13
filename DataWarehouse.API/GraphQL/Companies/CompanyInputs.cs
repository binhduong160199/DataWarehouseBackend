namespace DataWarehouse.API.GraphQL.Companies;

public record RegisterCompanyInput(string Name, string? Address, string? Website);
public record UpdateCompanyInput(Guid Id, string? Name, string? Address, string? Website);