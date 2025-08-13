namespace DataWarehouse.Models.DTOs
{
    public class RegisterUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
        public string? ProfileImageUrl { get; set; }
        public Guid? CompanyId { get; set; }
        public bool IsAdminCreating { get; set; } 
        public string Role { get; set; } = "User";
    }
}