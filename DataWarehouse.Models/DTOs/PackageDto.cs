namespace DataWarehouse.Models.DTOs
{
    public class PackageDto
    {
        public Guid Id { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public string PositionCode { get; set; } = string.Empty;
        public DateTime StoredAt { get; set; }
    }
}