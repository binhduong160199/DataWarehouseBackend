namespace DataWarehouse.Models.Entities
{
    public class Package
    {
        public Guid Id { get; set; }
        public Guid PartId { get; set; }
        public Part Part { get; set; } = null!;

        public Guid RackPositionId { get; set; }
        public RackPosition RackPosition { get; set; } = null!;

        public DateTime StoredAt { get; set; }
    }
}