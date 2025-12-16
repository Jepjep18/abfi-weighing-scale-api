using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace abfi_weighing_scale_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DBSets (tables)
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<ProductClassification> ProductClassification { get; set; }
        public DbSet<Production> Production { get; set; }
        public DbSet<ProductionFarm> ProductionFarms { get; set; }
        public DbSet<Farms> Farms { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingItem> BookingItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations automatically (Optional but clean)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
