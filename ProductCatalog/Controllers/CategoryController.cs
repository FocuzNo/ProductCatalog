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
            if(string.IsNullOrWhiteSpace(newCategory.CategoryName))
            {
                return BadRequest("The category name field is empty.");
            }

            return Ok(newCategory);
        }

        [HttpPut("EditCategory"), Authorize(Roles = "SuperUser")]
        public async Task<ActionResult> EditCategory(Category category)
        {
            await _categoryRepository.EditCategory(category);
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                return BadRequest("The category name field is empty.");
            }

            return Ok(category);
        }

        [HttpDelete("DeleteCategory/{id}"), Authorize(Roles = "SuperUser")]
        public async Task<ActionResult> DeleteCategory(int? id)
        {
            await _categoryRepository.DeleteCategory(id);
            return Ok();
        }

        [HttpGet("GetCategoryById/{id}"), Authorize(Roles = "SuperUser")]
        public async Task<ActionResult> GetProductById(int id)
        {
            var category = await _categoryRepository.GetCategoryById(id);
            return Ok(category);
        }
    }
}