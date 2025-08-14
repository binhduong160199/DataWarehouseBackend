using DataWarehouse.Models.DTOs;
using DataWarehouse.Models.Entities;

namespace DataWarehouse.API.Utils.Mapping
{
    public static class CompanyMapping
    {
        public static CompanyDto ToDto(Company c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            Website = c.Website,
            ContactEmail = c.ContactEmail,
            PhoneNumber = c.PhoneNumber,
            Industry = c.Industry,
            TaxId = c.TaxId,
            LogoUrl = c.LogoUrl,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }
}