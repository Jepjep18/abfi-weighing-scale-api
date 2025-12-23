namespace abfi_weighing_scale_api.Models.Entities
{
    public class BookingCustomerAdvance
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public decimal AdvanceAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentMethod { get; set; } // Optional: Cash, Card, etc.
        public string? ReferenceNumber { get; set; } // Optional: Transaction reference

        // Navigation properties
        public Booking Booking { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }
}
