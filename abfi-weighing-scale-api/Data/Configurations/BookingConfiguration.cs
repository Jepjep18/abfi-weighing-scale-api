using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.BookingDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.Property(b => b.Remarks)
                   .HasMaxLength(255);

            builder.Property(b => b.CreatedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasIndex(b => b.BookingDate)
                   .HasDatabaseName("IX_Bookings_BookingDate");
        }
    }
}
