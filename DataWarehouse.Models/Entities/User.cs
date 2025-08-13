using DataWarehouse.Models.Interfaces;
using DataWarehouse.Models.Enums;

namespace DataWarehouse.Models.Entities
{
    public class User : IUserIdentity
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        
        public UserRole Role { get; set; }
        
        Guid IUserIdentity.Id => Id;
        string IUserIdentity.Username => Username;
        string IUserIdentity.Role => Role.ToString();
    }
}