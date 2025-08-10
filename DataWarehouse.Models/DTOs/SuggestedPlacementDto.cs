namespace DataWarehouse.Models.DTOs
{
    public class SuggestedPlacementDto
    {
        public string SuggestedPositionCode { get; set; } = string.Empty;
        public float FitScore { get; set; }
    }
}