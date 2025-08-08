using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataWarehouse.API.Data.Configurations
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.ToTable("packages");
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => new { p.RackPositionId, p.PartId }).IsUnique();

            builder.HasOne(p => p.Part)
                .WithMany(pt => pt.Packages)
                .HasForeignKey(p => p.PartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.RackPosition)
                .WithOne(rp => rp.Package)
                .HasForeignKey<Package>(p => p.RackPositionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}