using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataWarehouse.API.Data.Configurations
{
    public class InventoryMismatchConfiguration : IEntityTypeConfiguration<InventoryMismatch>
    {
        public void Configure(EntityTypeBuilder<InventoryMismatch> builder)
        {
            builder.ToTable("inventory_mismatches");
            builder.HasKey(i => i.Id);
            builder.HasIndex(i => i.PartNumber);
        }
    }
}