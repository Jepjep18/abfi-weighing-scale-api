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
        public DbSet<Production> Productions { get; set; }
        public DbSet<Farms> Farms { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations automatically (Optional but clean)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
