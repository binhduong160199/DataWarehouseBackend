using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataWarehouse.API.Data.Configurations
{
    public class MovementLogConfiguration : IEntityTypeConfiguration<MovementLog>
    {
        public void Configure(EntityTypeBuilder<MovementLog> builder)
        {
            builder.ToTable("movement_logs");
            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.Package)
                .WithMany()
                .HasForeignKey(m => m.PackageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}