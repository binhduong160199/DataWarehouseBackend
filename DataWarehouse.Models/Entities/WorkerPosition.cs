namespace DataWarehouse.Models.Entities
{
    public class WorkerPosition
    {
        public Guid Id { get; set; }
        public Guid WorkerId { get; set; }
        public Worker Worker { get; set; } = null!;

        public string CurrentPositionCode { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}