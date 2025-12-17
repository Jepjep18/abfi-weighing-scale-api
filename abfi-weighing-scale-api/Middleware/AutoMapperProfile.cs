using abfi_weighing_scale_api.Controllers.Booking;
using abfi_weighing_scale_api.Models.Entities;
using AutoMapper;

namespace abfi_weighing_scale_api.Middleware
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // CreateBookingDto -> Booking
            CreateMap<CreateBookingDto, Booking>()
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate.Date))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Booking -> BookingResponseDto
            CreateMap<Booking, BookingResponseDto>()
                .ForMember(dest => dest.CustomerCount,
                           opt => opt.MapFrom(src => src.BookingItems
                                                        .Select(bi => bi.CustomerId)
                                                        .Distinct()
                                                        .Count()));
            //.ForMember(dest => dest.Items,
            //           opt => opt.MapFrom(src => src.BookingItems)); // maps BookingItem -> BookingItemResponseDto

            // BookingItem -> BookingItemResponseDto
            CreateMap<BookingItem, BookingItemResponseDto>()
                .ForPath(dest => dest.Customer.CustomerName, opt => opt.MapFrom(src => src.Customer.CustomerName))
                .ForPath(dest => dest.ProductClassification.ProductCode, opt => opt.MapFrom(src => src.ProductClassification.ProductCode))
                .ForMember(dest => dest.IsPrio, opt => opt.MapFrom(src => src.IsPrio));
        }
    }
}
