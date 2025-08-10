namespace DataWarehouse.Models.Entities
{
    public class RackPosition
    {
        public Guid Id { get; set; }
        public string PositionCode { get; set; } = string.Empty; // e.g., A1-1
        public int Level { get; set; } // floor level

        public Guid RackId { get; set; }
        public WarehouseRack Rack { get; set; } = null!;

        public Package? Package { get; set; }
    }
}