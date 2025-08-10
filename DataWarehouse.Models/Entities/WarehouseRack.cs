namespace DataWarehouse.Models.Entities
{
    public class WarehouseRack
    {
        public Guid Id { get; set; }
        public string RackCode { get; set; } = string.Empty; // e.g., A1, B2
        public ICollection<RackPosition> Positions { get; set; } = new List<RackPosition>();
    }
}