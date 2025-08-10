namespace DataWarehouse.Models.Interfaces
{
    public interface IUserIdentity
    {
        Guid Id { get; }
        string Username { get; }
        string Role { get; } 
    }
}