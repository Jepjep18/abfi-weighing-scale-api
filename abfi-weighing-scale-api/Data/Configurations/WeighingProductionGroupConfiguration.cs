using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class WeighingProductionGroupConfiguration : IEntityTypeConfiguration<WeighingProductionGroup>
    {
        public void Configure(EntityTypeBuilder<WeighingProductionGroup> builder)
        {
            builder.ToTable("WeighingProductionGroups");

            builder.HasKey(wpg => wpg.Id);

            // Required foreign key property
            builder.Property(wpg => wpg.ProductionId)
                   .IsRequired();

            // Default value for CreatedAt
            builder.Property(wpg => wpg.CreatedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .IsRequired();

            // Relationship with Production
            builder.HasOne(wpg => wpg.Production)
                   .WithMany(p => p.WeighingProductionGroups) // Reference the collection property
                   .HasForeignKey(wpg => wpg.ProductionId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Optional: Index for ProductionId for better query performance
            builder.HasIndex(wpg => wpg.ProductionId)
                   .HasDatabaseName("IX_WeighingProductionGroups_ProductionId");

            // Optional: Index for CreatedAt if you'll query by date
            builder.HasIndex(wpg => wpg.CreatedAt)
                   .HasDatabaseName("IX_WeighingProductionGroups_CreatedAt");
        }
    }
}