using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataWarehouse.API.Data.Configurations
{
    public class SuggestedPlacementConfiguration : IEntityTypeConfiguration<SuggestedPlacement>
    {
        public void Configure(EntityTypeBuilder<SuggestedPlacement> builder)
        {
            builder.ToTable("suggested_placements");
            builder.HasKey(sp => sp.Id);
            builder.HasIndex(sp => sp.PartId);
        }
    }
}