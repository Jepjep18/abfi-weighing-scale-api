using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class ProductionConfiguration : IEntityTypeConfiguration<Production>
    {
        public void Configure(EntityTypeBuilder<Production> builder)
        {
            // Add new relationship with ProductionFarm
            builder.HasMany(p => p.ProductionFarms)
                   .WithOne(pf => pf.Production)
                   .HasForeignKey(pf => pf.ProductionId)
                   .OnDelete(DeleteBehavior.Cascade); // When production is deleted, delete all associated ProductionFarm records

            // Configure properties
            builder.Property(p => p.ProductionName)
                   .HasMaxLength(200)
                   .IsRequired(false); // Make it optional if needed

            builder.Property(p => p.Description)
                   .HasMaxLength(500)
                   .IsRequired(false);

            // Add defaults for nullable properties
            builder.Property(p => p.TotalHeads)
                   .HasDefaultValue(0);

            // Configure DateTime properties
            builder.Property(p => p.StartDateTime)
                   .HasColumnType("datetime");

            builder.Property(p => p.EndDateTime)
                   .HasColumnType("datetime");

            builder.Property(p => p.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("GETUTCDATE()");

            // Add indexes for better query performance
            builder.HasIndex(p => p.ProductionName)
                   .HasDatabaseName("IX_Production_ProductionName");

            builder.HasIndex(p => p.StartDateTime)
                   .HasDatabaseName("IX_Production_StartDateTime");

            builder.HasIndex(p => p.EndDateTime)
                   .HasDatabaseName("IX_Production_EndDateTime");

            builder.HasIndex(p => p.CreatedAt)
                   .HasDatabaseName("IX_Production_CreatedAt");
        }
    }
}