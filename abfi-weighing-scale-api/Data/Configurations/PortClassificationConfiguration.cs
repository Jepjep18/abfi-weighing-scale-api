using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class PortClassificationConfiguration : IEntityTypeConfiguration<PortClassification>
    {
        public void Configure(EntityTypeBuilder<PortClassification> builder)
        {
            builder.ToTable("PortClassification");

            builder.HasKey(p => p.PortNumber);

            builder.Property(p => p.PortNumber)
                   .IsRequired()
                   .HasColumnType("nvarchar(50)")
                   .HasMaxLength(50);

            builder.Property(p => p.Class)
                   .HasColumnType("nvarchar(50)")
                   .HasMaxLength(50);
        }
    }
}
