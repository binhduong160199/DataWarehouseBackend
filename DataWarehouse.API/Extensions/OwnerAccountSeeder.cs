using DataWarehouse.API.Data;
using DataWarehouse.API.Utils.Hash;
using DataWarehouse.Models.Entities;
using DataWarehouse.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataWarehouse.API.Extensions;

public static class OwnerAccountSeeder
{
    public static async Task SeedOwnerAccountAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        var hash = scope.ServiceProvider.GetRequiredService<IHashUtility>();

        await using var db = await factory.CreateDbContextAsync();

        var ownerExists = await db.Users.AnyAsync(u => u.Role == UserRole.Owner);
        if (ownerExists) return;

        string? GetConfig(string key)
        {
            var value = config[$"OwnerAccount:{key}"];
            return string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "null" ? null : value.Trim();
        }

        var ownerUser = new User
        {
            Id = Guid.NewGuid(),
            Username = GetConfig("Username")!,
            PasswordHash = hash.HashPassword(GetConfig("Password")!),
            FirstName = "Binh Duong",
            LastName = "Nguyen",
            Email = GetConfig("Email"),
            PhoneNumber = GetConfig("PhoneNumber"),
            Birthday = DateTime.TryParse(GetConfig("Birthday"), out var bday)
                ? DateTime.SpecifyKind(bday, DateTimeKind.Utc)
                : null,
            JobTitle = GetConfig("JobTitle"),
            Department = GetConfig("Department"),
            ProfileImageUrl = GetConfig("ProfileImageUrl"),
            Role = UserRole.Owner,
            CompanyId = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            IsActive = true
        };

        db.Users.Add(ownerUser);
        await db.SaveChangesAsync();
        Console.WriteLine("Owner account seeded.");
    }
}