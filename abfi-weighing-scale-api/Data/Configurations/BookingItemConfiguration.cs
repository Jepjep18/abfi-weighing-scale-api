// Data/Configurations/BookingItemConfiguration.cs
using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace abfi_weighing_scale_api.Data.Configurations
{
    public class BookingItemConfiguration : IEntityTypeConfiguration<BookingItem>
    {
        public void Configure(EntityTypeBuilder<BookingItem> builder)
        {
            builder.ToTable("BookingItems");

            builder.HasKey(bi => bi.Id);

            builder.Property(bi => bi.Quantity)
                   .HasPrecision(12, 2)
                   .IsRequired();

            builder.Property(bi => bi.IsPrio) // NEW
                   .HasDefaultValue(false);

            builder.Property(bi => bi.CreatedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            // Relationships
            builder.HasOne(bi => bi.Booking)
                   .WithMany(b => b.BookingItems)
                   .HasForeignKey(bi => bi.BookingId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bi => bi.Customer)
                   .WithMany(c => c.BookingItems)
                   .HasForeignKey(bi => bi.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bi => bi.ProductClassification)
                   .WithMany(p => p.BookingItems)
                   .HasForeignKey(bi => bi.ProductClassificationId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Prevent duplicate Excel cells
            builder.HasIndex(bi => new
            {
                bi.BookingId,
                bi.CustomerId,
                bi.ProductClassificationId
            })
            .IsUnique()
            .HasDatabaseName("UX_BookingItems_Booking_Customer_Product");

            // Index for faster prio queries
            builder.HasIndex(bi => bi.IsPrio)
                   .HasDatabaseName("IX_BookingItems_IsPrio");
        }
    }
}