using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataWarehouse.API.Data.Configurations
{
    public class ScanSessionConfiguration : IEntityTypeConfiguration<ScanSession>
    {
        public void Configure(EntityTypeBuilder<ScanSession> builder)
        {
            builder.ToTable("scan_sessions");
            builder.HasKey(s => s.Id);
            builder.HasIndex(s => new { s.ScannedPositionCode, s.ScannedPartNumber });
        }
    }
}