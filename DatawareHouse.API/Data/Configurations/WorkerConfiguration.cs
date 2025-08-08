using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataWarehouse.API.Data.Configurations
{
    public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
    {
        public void Configure(EntityTypeBuilder<Worker> builder)
        {
            builder.ToTable("workers");
            builder.HasKey(w => w.Id);
            builder.HasIndex(w => w.EmployeeCode).IsUnique();
        }
    }
}