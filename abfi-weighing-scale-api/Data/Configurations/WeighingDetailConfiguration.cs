using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class WeighingDetailConfiguration : IEntityTypeConfiguration<WeighingDetail>
    {
        public void Configure(EntityTypeBuilder<WeighingDetail> builder)
        {
            builder.ToTable("WeighingDetails");

            builder.HasKey(w => w.id);

            builder.Property(w => w.id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            builder.Property(w => w.SerialData)
                   .IsRequired()
                   .HasColumnType("nvarchar(150)")
                   .HasMaxLength(150);

            builder.Property(w => w.Qty)
                   .HasColumnType("numeric(18,3)");

            builder.Property(w => w.UoM)
                   .HasColumnType("nvarchar(50)")
                   .HasMaxLength(50);

            builder.Property(w => w.Heads)
                   .IsRequired(false);

            builder.Property(w => w.ProdCode)
                   .HasColumnType("nvarchar(150)")
                   .HasMaxLength(150);

            builder.Property(w => w.CreatedDateTime)
                   .HasColumnType("datetime");

            builder.Property(w => w.PortNumber)
                   .HasColumnType("nvarchar(50)")
                   .HasMaxLength(50);

            builder.Property(w => w.Class)
                   .HasColumnType("nvarchar(150)")
                   .HasMaxLength(150);

            builder.Property(w => w.Remarks)
                   .HasColumnType("nvarchar(150)")
                   .HasMaxLength(150);

            builder.HasIndex(w => w.CreatedDateTime)
                   .HasDatabaseName("IX_WeighingDetails_CreatedDateTime");
        }
    }
}
