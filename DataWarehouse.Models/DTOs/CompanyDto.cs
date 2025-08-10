namespace DataWarehouse.Models.DTOs
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Website { get; set; }
    }

    public class CreateUpdateCompanyDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Website { get; set; }
    }
}