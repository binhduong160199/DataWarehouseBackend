namespace DataWarehouse.API.GraphQL.Users
{
    public record RegisterUserInput(
        string Username,
        string Password,
        string FirstName,
        string LastName,
        Guid CompanyId,
        string? Email,
        string? PhoneNumber,
        DateTime? Birthday,
        string? JobTitle,
        string? Department,
        string? ProfileImageUrl,
        string Role = "User"
    );

    public record LoginInput(string Username, string Password);
}