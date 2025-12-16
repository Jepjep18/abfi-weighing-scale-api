namespace abfi_weighing_scale_api.Models.Entities
{
    public class BookingItem
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public int ProductClassificationId { get; set; }

        public decimal Quantity { get; set; }
        public bool IsPrio { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Booking Booking { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public ProductClassification ProductClassification { get; set; } = null!;
    }
}
