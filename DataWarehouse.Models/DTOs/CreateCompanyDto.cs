namespace DataWarehouse.Models.DTOs;

public class CreateCompanyDto
{
    public string Name { get; set; } = string.Empty; 
    public string? Address { get; set; }
    public string? Website { get; set; }
    public string? ContactEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Industry { get; set; }
    public string? TaxId { get; set; }
    public string? LogoUrl { get; set; }
}