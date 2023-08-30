using Microsoft.EntityFrameworkCore;
using ProductCatalog.DAL.Entities;

namespace ProductCatalog.DAL
{
    public class DataContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public DataContext(DbContextOptions<DataContext> options)
           : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
            .HasMany(p => p.Products)
            .WithOne(p => p.Category).IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasKey(p => p.Id);

            modelBuilder.Entity<Category>().HasData(
            new Category[]
            {
                new Category { Id = 1, CategoryName = "Еда"},
                new Category { Id = 2, CategoryName = "Вкусности"},
                new Category { Id = 3, CategoryName = "Вода"},
            });

            modelBuilder.Entity<Product>().HasData(
                new Product[]
                {
                    new Product { Id = 1, ProductName = "Селедка", CategoryId = 1, ProductDescription = "Селедка соленая",
                    Price = 10.000M, GeneralNote = "Акция", SpecialNote = "Пересоленая"},

                    new Product { Id = 2, ProductName = "Тушенка", CategoryId = 1, ProductDescription = "Тушенка говяжая",
                    Price = 20.000M, GeneralNote = "Вкусная", SpecialNote = "Жилы"},

                    new Product { Id = 3, ProductName = "Сгущенка", CategoryId = 2, ProductDescription = "В банках",
                    Price = 30.000M, GeneralNote = "С ключом", SpecialNote = "Вкусная"},

                    new Product { Id = 4, ProductName = "Квас", CategoryId = 3, ProductDescription = "В бутылках",
                    Price = 15.000M, GeneralNote = "Вятский", SpecialNote = "Теплый"},
                });
        }
    }
}