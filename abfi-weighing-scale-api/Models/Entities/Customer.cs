namespace abfi_weighing_scale_api.Models.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = null!;
        public string? CustomerType { get; set; } // Distributor, Retailer, etc.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
    }
}
