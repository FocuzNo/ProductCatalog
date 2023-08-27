using ProductCatalog.DAL.Entities;
using ProductCatalog.DAL;
using ProductCatalog.Service.IRepository;

namespace ProductCatalog.Service.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _dataContext;
        public CategoryRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task AddCategory(Category category)
        {
            _dataContext.Add(category);
            await _dataContext.SaveChangesAsync();
        }

        public async Task EditCategory(Category category)
        {
            var categories = _dataContext.Categories.FirstOrDefault(q => q.Id == category.Id);
            if (categories != null)
            {
                categories.CategoryName = category.CategoryName;
                _dataContext.Categories.Update(categories);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task DeleteCategory(int? id)
        {
            var categories = _dataContext.Categories.FirstOrDefault(i => i.Id == id);
            if (categories != null)
            {
                _dataContext.Categories.Remove(categories);
                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
