namespace DataWarehouse.Models.DTOs
{
    public class MovePackageDto
    {
        public Guid PackageId { get; set; }
        public string NewPositionCode { get; set; } = string.Empty;
    }
}