using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.DAL;
using ProductCatalog.DAL.Entities;
using ProductCatalog.Service.IRepository;
namespace ProductCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly DataContext _dataContext;

        public ProductController(DataContext dataContext, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _dataContext = dataContext;
        }

        [HttpPost("AddProducts"), Authorize(Roles = "SuperUser, User")]
        public async Task<ActionResult> AddProduct(Product newProduct)
        {
            await _productRepository.AddProduct(newProduct);
            return Ok(newProduct);
        }

        [HttpPut("EditProducts"), Authorize(Roles = "SuperUser, User")]
        public async Task<ActionResult> EditProduct(Product product)
        {
            await _productRepository.EditProduct(product);
            return Ok(product);
        }

        [HttpDelete("DeleteProducts/{id}"), Authorize(Roles = "SuperUser")]
        public async Task<ActionResult> DeleteProduct(int? id)
        {
            await _productRepository.DeleteProduct(id);
            return Ok();
        }

        [HttpGet("GetProducts"), Authorize(Roles = "SuperUser")]
        public async Task <ActionResult> GetProducts()
        {
            var product = await _dataContext.Products.ToListAsync();
            return Ok(product);
        }

        [HttpGet("GetProductById/{id}"), Authorize(Roles = "SuperUser, User")]
        public async Task <ActionResult> GetProductById(int id)
        {
            var product = await _productRepository.GetProductById(id);
            return Ok(product);
        }

        [HttpGet("GetProductWithoutSpecial"), Authorize(Roles = "User")]
        public async Task <ActionResult> GetProductWithoutSpecial()
        {
            var product = await _productRepository.GetProductWithoutSpecial();
            return Ok(product);
        }

        [HttpGet("SearchByProduct/{searchBy}/{name}"), Authorize(Roles = "SuperUser, User")]
        public async Task<ActionResult> SearchByProduct(string? searchBy, string? name)
        {
           var productSearch = await _productRepository.SearchByProduct(searchBy, name);
           return Ok(productSearch);
        }
    }
}