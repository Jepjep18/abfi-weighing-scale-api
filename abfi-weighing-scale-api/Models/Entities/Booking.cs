namespace abfi_weighing_scale_api.Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public DateTime BookingDate { get; set; }
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
        public ICollection<BookingCustomerAdvance> CustomerAdvances { get; set; } = new List<BookingCustomerAdvance>();
    }
}
