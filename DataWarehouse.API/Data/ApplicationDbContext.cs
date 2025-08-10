using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataWarehouse.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {}

        public DbSet<WarehouseRack> WarehouseRacks => Set<WarehouseRack>();
        public DbSet<RackPosition> RackPositions => Set<RackPosition>();
        public DbSet<Part> Parts => Set<Part>();
        public DbSet<Package> Packages => Set<Package>();
        public DbSet<Worker> Workers => Set<Worker>();
        public DbSet<WorkerPosition> WorkerPositions => Set<WorkerPosition>();
        public DbSet<MovementLog> MovementLogs => Set<MovementLog>();
        public DbSet<ScanSession> ScanSessions => Set<ScanSession>();
        public DbSet<InventoryMismatch> InventoryMismatches => Set<InventoryMismatch>();
        public DbSet<SuggestedPlacement> SuggestedPlacements => Set<SuggestedPlacement>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Company> Companies => Set<Company>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}