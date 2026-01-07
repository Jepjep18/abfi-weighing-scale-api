using abfi_weighing_scale_api.Data;
using abfi_weighing_scale_api.Middleware;
using abfi_weighing_scale_api.Repositories.Implementations;
using abfi_weighing_scale_api.Repositories.Interfaces;
using abfi_weighing_scale_api.Services;
using abfi_weighing_scale_api.Services.Booking;
using abfi_weighing_scale_api.Services.Farms;
using abfi_weighing_scale_api.Services.Implementations;
using abfi_weighing_scale_api.Services.Interfaces;
using abfi_weighing_scale_api.Services.ProductClassifications;
using abfi_weighing_scale_api.Services.Production;
using abfi_weighing_scale_api.Services.WeighingDataProcessorService;
using abfi_weighing_scale_api.Services.WeighingService;
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

            // Booking Service
            services.AddScoped<IBookingService, BookingService>();

            // ===================== NEW WEIGHING SERVICES =====================
            // Weighing Data Processor (logic from GetProdClass stored procedure)
            services.AddScoped<IWeighingDataProcessor, WeighingDataProcessor>();

            // Weighing Service (main service for weighing operations)
            services.AddScoped<IWeighingService, WeighingService>();
            // ===================== END WEIGHING SERVICES =====================

            //weighing production group
            services.AddScoped<IWeighingProductionGroupService, WeighingProductionGroupService>();

            return services;
        }
    }
}