using DataWarehouse.Models.Enums;

namespace DataWarehouse.Models.Entities
{
    public class ScanSession
    {
        public Guid Id { get; set; }
        public string ScannedPositionCode { get; set; } = string.Empty;
        public string ScannedPartNumber { get; set; } = string.Empty;
        public ScanResultStatus Status { get; set; }
        public DateTime ScannedAt { get; set; }
    }
}