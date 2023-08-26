using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ProductCatalog.DAL.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ProductCatalog.DAL
{
    public class DataContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        public DataContext(DbContextOptions<DataContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
            .HasMany(p => p.Products)
            .WithOne(p => p.Category).IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
