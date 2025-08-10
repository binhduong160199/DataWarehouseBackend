using DataWarehouse.Models.Enums;

namespace DataWarehouse.Models.DTOs
{
    public class InventoryCheckDto
    {
        public string PartNumber { get; set; } = string.Empty;
        public string? DetectedPositionCode { get; set; }
        public MismatchType MismatchType { get; set; }
        public DateTime DetectedAt { get; set; }
    }
}
