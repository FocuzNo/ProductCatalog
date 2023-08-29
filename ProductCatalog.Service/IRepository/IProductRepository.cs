using ProductCatalog.DAL.Entities;

namespace ProductCatalog.Service.IRepository
{
    public interface IProductRepository
    {
        Task AddProduct(Product product);
        Task EditProduct(Product product);
        Task DeleteProduct(int? id);
        Task <List<Product>> GetProducts();
        Task<Product?> GetProductById(int? id);
        Task<List<Product>> GetProductWithoutSpecial();
        Task<IEnumerable<Product>> SearchByProduct(string? searchBy, string? name);
    }
}
