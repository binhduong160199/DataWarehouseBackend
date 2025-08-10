using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataWarehouse.API.Data.Configurations
{
    public class WarehouseRackConfiguration : IEntityTypeConfiguration<WarehouseRack>
    {
        public void Configure(EntityTypeBuilder<WarehouseRack> builder)
        {
            builder.ToTable("warehouse_racks");
            builder.HasKey(r => r.Id);
        }
    }
}