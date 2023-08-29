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
    public class ProductController : Controller
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

        [HttpDelete("DeleteProducts/{id}"), Authorize(Roles = "")]
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

            //var products = await _dataContext.Products.Join(_dataContext.Categories,
            //p => p.CategoryId,
            //c => c.Id,
            //(p, c) => new
            //{
            //    ProductName = p.ProductName,
            //    CategoryName = c.CategoryName,
            //    ProductDescription = p.ProductDescription,
            //    SpecialNote = p.SpecialNote,
            //    Price = p.Price,
            //    GeneralNote = p.GeneralNote,
            //}).ToListAsync();


            //return Ok(products);
        }

        [HttpGet("GetProductById/{id}"), Authorize(Roles = "SuperUser")]
        public async Task <ActionResult> GetProductById(int id)
        {
            var product = await _productRepository.GetProductById(id);
            return Ok(product);
        }
    }
}