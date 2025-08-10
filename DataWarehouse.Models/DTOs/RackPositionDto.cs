namespace DataWarehouse.Models.DTOs
{
    public class RackPositionDto
    {
        public Guid Id { get; set; }
        public string PositionCode { get; set; } = string.Empty;
        public int Level { get; set; }
    }
}