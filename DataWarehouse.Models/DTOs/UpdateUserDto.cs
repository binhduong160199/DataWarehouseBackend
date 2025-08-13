namespace DataWarehouse.Models.DTOs
{
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? NewPassword { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}