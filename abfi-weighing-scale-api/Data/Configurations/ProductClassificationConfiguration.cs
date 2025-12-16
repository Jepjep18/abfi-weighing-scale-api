using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class ProductClassificationConfiguration : IEntityTypeConfiguration<ProductClassification>
    {
        public void Configure(EntityTypeBuilder<ProductClassification> builder)
        {
            builder.ToTable("ProductClassification");

            builder.HasKey(p => p.Id);

            // Fix decimal precision warning
            builder.Property(p => p.CratesWeight)
                   .HasPrecision(18, 2);

            builder.Property(p => p.ProductCode)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(p => p.IndividualWeightRange)
                   .HasMaxLength(100);

            builder.Property(p => p.TotalWeightRangePerCrate)
                   .HasMaxLength(100);

            builder.Property(p => p.CreatedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            // Index for fast lookup during Excel import
            builder.HasIndex(p => p.ProductCode)
                   .HasDatabaseName("IX_ProductClassification_ProductCode");

            builder.HasIndex(p => p.IsActive)
                   .HasDatabaseName("IX_ProductClassification_IsActive");
        }
    }
}
