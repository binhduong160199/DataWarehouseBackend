using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataWarehouse.API.Data.Configurations
{
    public class RackPositionConfiguration : IEntityTypeConfiguration<RackPosition>
    {
        public void Configure(EntityTypeBuilder<RackPosition> builder)
        {
            builder.ToTable("rack_positions");
            builder.HasKey(rp => rp.Id);
            builder.HasIndex(rp => rp.PositionCode).IsUnique();

            builder.HasOne(rp => rp.Rack)
                .WithMany(r => r.Positions)
                .HasForeignKey(rp => rp.RackId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}