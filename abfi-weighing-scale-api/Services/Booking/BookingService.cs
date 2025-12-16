using abfi_weighing_scale_api.Controllers.Booking;
using abfi_weighing_scale_api.Data;
using abfi_weighing_scale_api.Exceptions;
using abfi_weighing_scale_api.Models.Dtos;
using abfi_weighing_scale_api.Models.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace abfi_weighing_scale_api.Services.Booking
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<BookingResponseDto> GetBookingByIdAsync(int id);
        Task<IEnumerable<BookingResponseDto>> GetBookingsByDateAsync(DateTime date);
        Task<IEnumerable<BookingResponseDto>> GetTodayBookingsAsync();
        Task<PagedResponseDto<BookingResponseDto>> GetBookingsAsync(BookingRequestDto request);

    }

    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            AppDbContext context,
            IMapper mapper,
            ILogger<BookingService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto createBookingDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Creating new booking...");

                // 1. Create Booking entity - Use FULLY QUALIFIED NAME
                var booking = _mapper.Map<Models.Entities.Booking>(createBookingDto);
                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Booking created with ID: {booking.Id}");

                // 2. Process each customer item
                foreach (var itemDto in createBookingDto.Items)
                {
                    await ProcessBookingItemAsync(booking.Id, itemDto);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Successfully created booking {booking.Id}");

                // 3. Return the created booking
                return await GetBookingByIdAsync(booking.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating booking");
                throw new Exception($"Failed to create booking: {ex.Message}");
            }
        }

        private async Task ProcessBookingItemAsync(int bookingId, BookingItemDto itemDto)
        {
            // Get or create customer
            var customer = await GetOrCreateCustomerAsync(itemDto.CustomerName);

            // Process each product quantity for this customer
            foreach (var productQuantity in itemDto.ProductQuantities)
            {
                if (productQuantity.Value <= 0)
                {
                    _logger.LogWarning($"Skipping zero quantity for {itemDto.CustomerName}");
                    continue;
                }

                // Verify product exists
                var productExists = await _context.ProductClassification
                    .AnyAsync(p => p.Id == productQuantity.Key);

                if (!productExists)
                {
                    _logger.LogWarning($"Product with ID {productQuantity.Key} not found");
                    continue;
                }

                // Create booking item
                var bookingItem = new BookingItem
                {
                    BookingId = bookingId,
                    CustomerId = customer.Id,
                    ProductClassificationId = productQuantity.Key,
                    Quantity = productQuantity.Value,
                    IsPrio = itemDto.IsPrio,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.BookingItems.AddAsync(bookingItem);

                _logger.LogDebug($"Created booking item - Customer: {customer.CustomerName}, " +
                                $"ProductId: {productQuantity.Key}, Quantity: {productQuantity.Value}");
            }
        }

        private async Task<Customer> GetOrCreateCustomerAsync(string customerName)
        {
            var normalizedName = customerName.Trim().ToLower();

            var customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.CustomerName.ToLower() == normalizedName);

            if (customer == null)
            {
                customer = new Customer
                {
                    CustomerName = customerName.Trim(),
                    CustomerType = "Distributor",
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Customer.AddAsync(customer);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created new customer: {customerName}");
            }

            return customer;
        }

        public async Task<BookingResponseDto> GetBookingByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.Customer)
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.ProductClassification)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                _logger.LogWarning($"Booking with ID {id} not found");
                throw new NotFoundException($"Booking with ID {id} not found");
            }

            var response = _mapper.Map<BookingResponseDto>(booking);

            // Calculate totals
            //response.TotalQuantity = booking.BookingItems.Sum(bi => bi.Quantity);
            //response.TotalAmount = booking.BookingItems.Sum(bi => bi.Quantity) * 146;

            return response;
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingsByDateAsync(DateTime date)
        {
            var bookings = await _context.Bookings
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.Customer)
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.ProductClassification)
                .Where(b => b.BookingDate.Date == date.Date)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var responses = new List<BookingResponseDto>();

            foreach (var booking in bookings)
            {
                var response = _mapper.Map<BookingResponseDto>(booking);
                //response.TotalQuantity = booking.BookingItems.Sum(bi => bi.Quantity);
                //response.TotalAmount = booking.BookingItems.Sum(bi => bi.Quantity) * 146;
                responses.Add(response);
            }

            return responses;
        }

        public async Task<PagedResponseDto<BookingResponseDto>> GetBookingsAsync(BookingRequestDto request)
        {
            request.PageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            request.PageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

            var query = _context.Bookings
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.Customer)
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.ProductClassification)
                .AsQueryable();

            // Optional search filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(b =>
                    b.BookingItems.Any(bi =>
                        bi.Customer.CustomerName.ToLower().Contains(searchTerm) ||
                        bi.ProductClassification.ProductCode.ToLower().Contains(searchTerm)
                    )
                );
            }

            // Optional date filter
            if (request.BookingDate.HasValue)
            {
                var date = request.BookingDate.Value.Date;
                query = query.Where(b => b.BookingDate.Date == date);
            }

            var totalCount = await query.CountAsync();

            var bookings = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var items = bookings.Select(b =>
            {
                var dto = _mapper.Map<BookingResponseDto>(b);

                // Instead of total quantity/amount, count unique customers
                dto.CustomerCount = b.BookingItems.Select(bi => bi.CustomerId).Distinct().Count();

                

                return dto;
            });


            return new PagedResponseDto<BookingResponseDto>
            {
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Items = items
            };
        }


        public async Task<IEnumerable<BookingResponseDto>> GetTodayBookingsAsync()
        {
            return await GetBookingsByDateAsync(DateTime.Today);
        }
    }
}
