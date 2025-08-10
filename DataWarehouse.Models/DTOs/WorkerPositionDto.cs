namespace DataWarehouse.Models.DTOs
{
    public class WorkerPositionDto
    {
        public Guid WorkerId { get; set; }
        public string CurrentPositionCode { get; set; } = string.Empty;
    }
}