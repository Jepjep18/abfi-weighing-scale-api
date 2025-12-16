using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CustomerName)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(c => c.CustomerType)
                   .HasMaxLength(50);

            builder.Property(c => c.CreatedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasIndex(c => c.CustomerName)
                   .HasDatabaseName("IX_Customers_CustomerName");
        }
    }
}
