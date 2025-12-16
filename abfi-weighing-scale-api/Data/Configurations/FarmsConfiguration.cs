using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class FarmsConfiguration : IEntityTypeConfiguration<Farms>
    {
        public void Configure(EntityTypeBuilder<Farms> builder)
        {
            // Primary key
            builder.HasKey(f => f.Id);

            // Configure properties
            builder.Property(f => f.FarmName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(f => f.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(f => f.UpdatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("GETUTCDATE()");

            // Unique constraint on FarmName
            builder.HasIndex(f => f.FarmName)
                   .IsUnique()
                   .HasDatabaseName("IX_Farms_FarmName");

            // Configure relationship with ProductionFarm
            builder.HasMany(f => f.ProductionFarms)
                   .WithOne(pf => pf.Farm)
                   .HasForeignKey(pf => pf.FarmId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Add indexes
            builder.HasIndex(f => f.IsActive)
                   .HasDatabaseName("IX_Farms_IsActive");
        }
    }
}
