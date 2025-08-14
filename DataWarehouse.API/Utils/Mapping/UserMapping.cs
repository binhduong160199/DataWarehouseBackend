using DataWarehouse.Models.DTOs;
using DataWarehouse.Models.Entities;

namespace DataWarehouse.API.Utils.Mapping
{
    public static class UserMapping
    {
        public static UserProfileDto ToProfile(User u) => new()
        {
            Id = u.Id,
            Username = u.Username,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            Birthday = u.Birthday,
            JobTitle = u.JobTitle,
            Department = u.Department,
            ProfileImageUrl = u.ProfileImageUrl,
            LastLoginAt = u.LastLoginAt,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
            CompanyId = u.CompanyId,
            CompanyName = u.Company?.Name ?? string.Empty,
            Role = u.Role.ToString()
        };
    }
}