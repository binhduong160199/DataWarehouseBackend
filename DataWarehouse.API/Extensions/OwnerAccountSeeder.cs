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

        var ownerUsername = config["OwnerAccount:Username"];
        var ownerPassword = config["OwnerAccount:Password"];
        var ownerEmail = config["OwnerAccount:Email"];

        var ownerUser = new User
        {
            Id = Guid.NewGuid(),
            Username = ownerUsername,
            PasswordHash = hash.HashPassword(ownerPassword!),
            FirstName = "Binh Duong",
            LastName = "Nguyen",
            Email = ownerEmail,
            Role = UserRole.Owner,
            CompanyId = null
        };

        db.Users.Add(ownerUser);
        await db.SaveChangesAsync();
        Console.WriteLine("Owner account seeded.");
    }
}