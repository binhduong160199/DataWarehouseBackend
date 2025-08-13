using DataWarehouse.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataWarehouse.API.Data.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("companies");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired();

            builder.Property(c => c.ContactEmail).HasMaxLength(255);
            builder.Property(c => c.PhoneNumber).HasMaxLength(50);
            builder.Property(c => c.Industry).HasMaxLength(100);
            builder.Property(c => c.TaxId).HasMaxLength(50);
            builder.Property(c => c.LogoUrl).HasMaxLength(2048);

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);
        }
    }
}