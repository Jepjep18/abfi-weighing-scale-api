using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class ProductionConfiguration : IEntityTypeConfiguration<Production>
    {
        public void Configure(EntityTypeBuilder<Production> builder)
        {
            // Configure the Farm relationship
            builder.HasOne(p => p.Farm)
                   .WithMany() // Use .WithMany(f => f.Productions) if you add the navigation property in Farms
                   .HasForeignKey(p => p.FarmId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevents deleting a farm if it has productions

            // FarmId is required
            builder.Property(p => p.FarmId)
                   .IsRequired();

            // Add defaults for nullable properties
            builder.Property(p => p.ForcastedTrips)
                   .HasDefaultValue(0);

            builder.Property(p => p.NoOfHeads)
                   .HasDefaultValue(0);

            // Configure DateTime properties
            builder.Property(p => p.StartDateTime)
                   .HasColumnType("datetime");

            builder.Property(p => p.EndDateTime)
                   .HasColumnType("datetime");

            // Optional: Add indexes for better query performance
            builder.HasIndex(p => p.FarmId)
                   .HasDatabaseName("IX_Production_FarmId");

            builder.HasIndex(p => p.StartDateTime)
                   .HasDatabaseName("IX_Production_StartDateTime");

            builder.HasIndex(p => p.EndDateTime)
                   .HasDatabaseName("IX_Production_EndDateTime");
        }
    }
}