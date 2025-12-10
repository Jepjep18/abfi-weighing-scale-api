using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class ProductClassificationConfiguration : IEntityTypeConfiguration<ProductClassification>
    {
        public void Configure(EntityTypeBuilder<ProductClassification> builder)
        {
            // Fix decimal precision warning
            builder.Property(p => p.CratesWeight)
                   .HasPrecision(18, 2);

            // Optional: Add constraints if needed
            builder.Property(p => p.ProductCode).HasMaxLength(100);
            builder.Property(p => p.IndividualWeightRange).HasMaxLength(100);
            builder.Property(p => p.TotalWeightRangePerCrate).HasMaxLength(100);

            // You may also set default values
            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
