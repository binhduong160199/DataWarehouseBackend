using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataWarehouse.API.Data.Configurations
{
    public class WorkerPositionConfiguration : IEntityTypeConfiguration<WorkerPosition>
    {
        public void Configure(EntityTypeBuilder<WorkerPosition> builder)
        {
            builder.ToTable("worker_positions");
            builder.HasKey(wp => wp.Id);

            builder.HasOne(wp => wp.Worker)
                .WithMany()
                .HasForeignKey(wp => wp.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}