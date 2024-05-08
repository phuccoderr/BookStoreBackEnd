using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBook.Data;
using ProjectBook.Models;
using ProjectBook.Serivces;

namespace ProjectBook.Controllers.Frontend
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsCusController(ApiDbContext dbContext) : ControllerBase
    {
        private CategoryService categoryService = new CategoryService(dbContext);
        private ProductService productService = new ProductService(dbContext);


        [HttpGet("/categories")]
        public IActionResult ListCategories()
        {
            return Ok(categoryService.liSTCategories());
        }

        [HttpGet("/c/{alias}/page/{pageNum}")]
        public IActionResult ListProduct(string sort, string keyword,string alias,int pageNum = 1)
        {
            var cateId = categoryService.getCategory(alias);
            var listProducts = productService.listByCategory(pageNum, cateId,sort, keyword);
            return Ok(listProducts);
        }

        [HttpGet("/p/{alias}")]
        public IActionResult GetProduct(string alias)
        {
            var currentProduct = productService.getProductByAlias(alias);
            if (currentProduct == null )
            {
                return NotFound();
            }

            return Ok(currentProduct);
        }

    }
}
