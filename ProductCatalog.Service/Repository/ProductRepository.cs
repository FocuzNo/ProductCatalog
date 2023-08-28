using ProductCatalog.DAL.Entities;
using ProductCatalog.DAL;
using ProductCatalog.Service.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ProductCatalog.Service.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _dataContext;
        public ProductRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task AddProduct(Product product)
        {
            _dataContext.Add(product);
            await _dataContext.SaveChangesAsync();
        }

        public async Task EditProduct(Product product)
        {
            var products = _dataContext.Products.FirstOrDefault(q => q.Id == product.Id);
            if (products != null)
            {
                products.ProductName = product.ProductName;
                products.ProductDescription = product.ProductDescription;
                products.SpecialNote = product.SpecialNote;
                products.Price = product.Price;
                products.GeneralNote = product.GeneralNote;

                _dataContext.Products.Update(products);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task DeleteProduct(int? id)
        {
            var products = _dataContext.Products.FirstOrDefault(i => i.Id == id);
            if (products != null)
            {
                _dataContext.Products.Remove(products);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> GetProducts()
        {
            var product = await _dataContext.Products.ToListAsync();
            return product;
           
        }

        public async Task<Product?> GetProductById(int? id)
        {
            Product? product = await _dataContext.Products.FirstOrDefaultAsync(u => u.Id == id);
            return product;
        }
    }
}
