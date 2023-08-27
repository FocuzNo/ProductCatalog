using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.DAL.Entities;
using ProductCatalog.Service.IRepository;

namespace ProductCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost("AddProducts"), Authorize(Roles = "SuperUser, User")]
        public async Task<ActionResult> AddCategory(Product newProduct)
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

        [HttpDelete("DeleteProducts"), Authorize(Roles = "SuperUser")]
        public async Task<ActionResult> DeleteProduct(int? id)
        {
            await _productRepository.DeleteProduct(id);
            return Ok();
        }
    }
}