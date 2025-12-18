using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class ProdClassificationConfiguration : IEntityTypeConfiguration<ProdClassification>
    {
        public void Configure(EntityTypeBuilder<ProdClassification> builder)
        {
            builder.ToTable("ProdClassification");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .HasColumnName("ID")
                   .ValueGeneratedOnAdd();

            builder.Property(p => p.ProdCode)
                   .HasColumnName("ProdCode")
                   .HasColumnType("nvarchar(150)")
                   .HasMaxLength(150);

            builder.Property(p => p.IndvWeight_Min)
                   .HasColumnName("IndvWeight_Min")
                   .HasColumnType("numeric(18,3)");

            builder.Property(p => p.IndvWeight_Max)
                   .HasColumnName("IndvWeight_Max")
                   .HasColumnType("numeric(18,3)");

            builder.Property(p => p.TotalIndvWeight_Min)
                   .HasColumnName("TotalIndvWeight_Min")
                   .HasColumnType("numeric(18,3)");

            builder.Property(p => p.TotalIndvWeight_Max)
                   .HasColumnName("TotalIndvWeight_Max")
                   .HasColumnType("numeric(18,3)");

            builder.Property(p => p.CratesWeight_Min)
                   .HasColumnName("CratesWeight_Min")
                   .HasColumnType("numeric(18,3)");

            builder.Property(p => p.CratesWeight_Max)
                   .HasColumnName("CratesWeight_Max")
                   .HasColumnType("numeric(18,3)");

            builder.Property(p => p.NumHeads)
                   .HasColumnName("NumHeads");

            builder.Property(p => p.Class)
                   .HasColumnName("Class")
                   .HasColumnType("nvarchar(50)")
                   .HasMaxLength(50);

            builder.Property(p => p.UoMAll)
                   .HasColumnName("UoMAll")
                   .HasColumnType("nvarchar(50)")
                   .HasMaxLength(50);

            // Add index for better query performance
            builder.HasIndex(p => new { p.Class, p.TotalIndvWeight_Min, p.TotalIndvWeight_Max })
                   .HasDatabaseName("IX_ProdClassification_Class_WeightRange");
        }
    }
}