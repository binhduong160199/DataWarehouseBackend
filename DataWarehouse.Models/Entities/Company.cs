namespace DataWarehouse.Models.Entities
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Website { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}