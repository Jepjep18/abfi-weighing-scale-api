namespace abfi_weighing_scale_api.Controllers.Booking
{
    public class CreateBookingDto
    {
        public DateTime BookingDate { get; set; }
        public string? Remarks { get; set; }
        public List<BookingItemDto> Items { get; set; } = new();
    }

    public class BookingItemDto
    {
        public string CustomerName { get; set; } = null!;
        public bool IsPrio { get; set; }
        public Dictionary<int, decimal> ProductQuantities { get; set; } = new();
    }

    public class BookingResponseDto
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        //public decimal TotalQuantity { get; set; }
        //public decimal TotalAmount { get; set; }
        public int CustomerCount { get; set; } // <-- NEW

        //public List<BookingItemResponseDto> Items { get; set; } = new();
    }

    public class BookingItemResponseDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public decimal Quantity { get; set; }
        public bool IsPrio { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BookingRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; } // e.g., customer name or product name
        public DateTime? BookingDate { get; set; } // optional filter by date
    }

}
