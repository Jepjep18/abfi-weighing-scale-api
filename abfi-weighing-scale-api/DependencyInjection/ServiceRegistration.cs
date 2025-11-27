using abfi_weighing_scale_api.Data;
using abfi_weighing_scale_api.Repositories.Implementations;
using abfi_weighing_scale_api.Repositories.Interfaces;
using abfi_weighing_scale_api.Services.Interfaces;
using abfi_weighing_scale_api.Services.Implementations;
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

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // Register Services
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
