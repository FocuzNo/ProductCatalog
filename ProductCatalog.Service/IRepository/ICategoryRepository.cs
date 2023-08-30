using ProductCatalog.DAL.Entities;

namespace ProductCatalog.Service.IRepository
{
    public interface ICategoryRepository
    {
        Task AddCategory(Category category);
        Task EditCategory(Category category);
        Task DeleteCategory(int? id);
        Task<Category?> GetCategoryById(int? id);
    }
}