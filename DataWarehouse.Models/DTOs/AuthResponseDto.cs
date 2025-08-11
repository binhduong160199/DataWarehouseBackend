namespace DataWarehouse.Models.DTOs
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public string? RefreshToken { get; set; }
        public UserProfileDto User { get; set; } = new();
    }
}