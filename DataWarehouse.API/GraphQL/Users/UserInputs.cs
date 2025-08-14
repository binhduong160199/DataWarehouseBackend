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

    public record UpdateUserInput(
        string? FirstName,
        string? LastName,
        string? Email,
        string? PhoneNumber,
        DateTime? Birthday,
        string? JobTitle,
        string? Department,
        string? ProfileImageUrl,
        string? Role,
        string? NewPassword
    );
    
    public record LoginInput(string Username, string Password);
}