namespace DataWarehouse.Models.DTOs
{
    public class MovementLogDto
    {
        public Guid PackageId { get; set; }
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}