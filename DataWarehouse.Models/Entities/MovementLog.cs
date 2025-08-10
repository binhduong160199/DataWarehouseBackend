namespace DataWarehouse.Models.Entities
{
    public class MovementLog
    {
        public Guid Id { get; set; }
        public Guid PackageId { get; set; }
        public Package Package { get; set; } = null!;

        public string FromPositionCode { get; set; } = string.Empty;
        public string ToPositionCode { get; set; } = string.Empty;
        public DateTime MovedAt { get; set; }
    }
}