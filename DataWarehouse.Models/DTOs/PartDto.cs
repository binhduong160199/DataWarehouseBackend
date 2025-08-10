namespace DataWarehouse.Models.DTOs
{
    public class PartDto
    {
        public Guid Id { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public string PartName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Depth { get; set; }
        public float Weight { get; set; }
    }
}