using DataWarehouse.Models.Enums;

namespace DataWarehouse.Models.Entities
{
    public class Part
    {
        public Guid Id { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public string PartName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Depth { get; set; }
        public float Weight { get; set; }
        public PartCategory Category { get; set; }

        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}