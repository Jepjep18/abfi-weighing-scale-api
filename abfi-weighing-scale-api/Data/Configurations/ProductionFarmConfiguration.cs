using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class ProductionFarmConfiguration : IEntityTypeConfiguration<ProductionFarm>
    {
        public void Configure(EntityTypeBuilder<ProductionFarm> builder)
        {
            // Primary key
            builder.HasKey(pf => pf.Id);

            // Configure Farm relationship
            builder.HasOne(pf => pf.Farm)
                   .WithMany(f => f.ProductionFarms)
                   .HasForeignKey(pf => pf.FarmId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevents deleting a farm if it has productions

            // Configure Production relationship
            builder.HasOne(pf => pf.Production)
                   .WithMany(p => p.ProductionFarms)
                   .HasForeignKey(pf => pf.ProductionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: A farm can only be added once to a production
            builder.HasIndex(pf => new { pf.ProductionId, pf.FarmId })
                   .IsUnique()
                   .HasDatabaseName("IX_ProductionFarm_ProductionId_FarmId");

            // Add defaults for nullable properties
            builder.Property(pf => pf.ForecastedTrips)
                   .HasDefaultValue(0);

            builder.Property(pf => pf.AllocatedHeads)
                   .HasDefaultValue(0);

            builder.Property(pf => pf.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("GETUTCDATE()");

            // Add indexes for better query performance
            builder.HasIndex(pf => pf.ProductionId)
                   .HasDatabaseName("IX_ProductionFarm_ProductionId");

            builder.HasIndex(pf => pf.FarmId)
                   .HasDatabaseName("IX_ProductionFarm_FarmId");
        }
    }
}
