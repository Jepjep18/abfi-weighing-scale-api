using abfi_weighing_scale_api.Exceptions;
using abfi_weighing_scale_api.Services.Booking;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace abfi_weighing_scale_api.Controllers.Booking
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(
            IBookingService bookingService,
            ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] CreateBookingDto createBookingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _logger.LogInformation("Received booking creation request");

                // Basic validation
                if (createBookingDto == null)
                    return BadRequest(new { message = "Booking data is required" });

                if (createBookingDto.Items == null || !createBookingDto.Items.Any())
                    return BadRequest(new { message = "At least one booking item is required" });

                // Validate each item
                var validationErrors = new List<string>();
                for (int i = 0; i < createBookingDto.Items.Count; i++)
                {
                    var item = createBookingDto.Items[i];

                    if (string.IsNullOrWhiteSpace(item.CustomerName))
                        validationErrors.Add($"Item {i + 1}: Customer name is required");

                    if (item.ProductQuantities == null || !item.ProductQuantities.Any())
                        validationErrors.Add($"Item {i + 1}: Product quantities required for customer: {item.CustomerName}");
                    else if (item.ProductQuantities.Values.Any(qty => qty < 0))
                        validationErrors.Add($"Item {i + 1}: Quantity cannot be negative");
                }

                if (validationErrors.Any())
                    return BadRequest(new { errors = validationErrors });

                var booking = await _bookingService.CreateBookingAsync(createBookingDto);

                _logger.LogInformation($"Successfully created booking with ID: {booking.Id}");

                return CreatedAtAction(
                    nameof(GetBooking),
                    new { id = booking.Id },
                    booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the booking",
                    detail = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings([FromQuery] BookingRequestDto request)
        {
            try
            {
                var result = await _bookingService.GetBookingsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings");
                return StatusCode(500, new { message = "An error occurred while retrieving bookings" });
            }
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookingResponseDto>> GetBooking(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid booking ID" });

            try
            {
                _logger.LogInformation($"Retrieving booking with ID: {id}");

                var booking = await _bookingService.GetBookingByIdAsync(id);
                return Ok(booking);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning($"Booking not found: {id}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving booking with ID: {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving the booking" });
            }
        }

        [HttpGet("date/{date:datetime}")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookingsByDate(DateTime date)
        {
            try
            {
                _logger.LogInformation($"Retrieving bookings for date: {date:yyyy-MM-dd}");

                var bookings = await _bookingService.GetBookingsByDateAsync(date);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving bookings for date: {date}");
                return StatusCode(500, new { message = "An error occurred while retrieving bookings" });
            }
        }

        [HttpGet("today")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetTodayBookings()
        {
            try
            {
                _logger.LogInformation("Retrieving today's bookings");

                var bookings = await _bookingService.GetTodayBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving today's bookings");
                return StatusCode(500, new { message = "An error occurred while retrieving bookings" });
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "Healthy",
                service = "Booking Service",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
