namespace DataWarehouse.API.GraphQL.Companies;

public record RegisterCompanyInput(
    string Name,
    string? Address,
    string? Website,
    string? ContactEmail,
    string? PhoneNumber,
    string? Industry,
    string? TaxId,
    string? LogoUrl,
    bool? IsActive
);

public record UpdateCompanyInput(
    Guid Id,
    string? Name,
    string? Address,
    string? Website,
    string? ContactEmail,
    string? PhoneNumber,
    string? Industry,
    string? TaxId,
    string? LogoUrl,
    bool? IsActive
);