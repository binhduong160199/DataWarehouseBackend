using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataWarehouse.API.Data.Configurations
{
    public class PartConfiguration : IEntityTypeConfiguration<Part>
    {
        public void Configure(EntityTypeBuilder<Part> builder)
        {
            builder.ToTable("parts");
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.PartNumber).IsUnique();
        }
    }
}