using abfi_weighing_scale_api.Data;
using abfi_weighing_scale_api.Middleware;
using abfi_weighing_scale_api.Repositories.Implementations;
using abfi_weighing_scale_api.Repositories.Interfaces;
using abfi_weighing_scale_api.Services.Booking;
using abfi_weighing_scale_api.Services.Farms;
using abfi_weighing_scale_api.Services.Implementations;
using abfi_weighing_scale_api.Services.Interfaces;
using abfi_weighing_scale_api.Services.ProductClassifications;
using abfi_weighing_scale_api.Services.Production;
using Microsoft.EntityFrameworkCore;

namespace abfi_weighing_scale_api.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Add AutoMapper - IMPORTANT!
            services.AddAutoMapper(typeof(AutoMapperProfile));

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // Register Services
            services.AddScoped<IUserService, UserService>();

            // Register ProductClassification Service
            services.AddScoped<IProductClassificationService, ProductClassificationService>();

            // Register Farm Service
            services.AddScoped<IFarmsService, FarmsService>();

            // Production Service
            services.AddScoped<IProductionService, ProductionService>();

            //booking Service
            services.AddScoped<IBookingService, BookingService>();

            return services;
        }
    }
}
