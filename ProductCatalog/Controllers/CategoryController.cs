using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.DAL.Entities;
using ProductCatalog.Service.IRepository;

namespace ProductCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpPost("AddCategory"), Authorize(Roles = "SuperUser")]
        public async Task<ActionResult> AddCategory(Category newCategory)
        {
            await _categoryRepository.AddCategory(newCategory);
            return Ok(newCategory);
        }

        [HttpPut("EditCategory"), Authorize(Roles = "SuperUser")]
        public async Task<ActionResult> EditCategory(Category category)
        {
            await _categoryRepository.EditCategory(category);
            return Ok(category);
        }

        [HttpDelete("DeleteCategory"), Authorize(Roles = "SuperUser")]
        public async Task<ActionResult> DeleteCategory(int? id)
        {
            await _categoryRepository.DeleteCategory(id);
            return Ok();
        }
    }
}