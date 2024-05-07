using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectBook.Data;
using ProjectBook.DTO;
using ProjectBook.Helpers;
using ProjectBook.Models;
using ProjectBook.Request;
using ProjectBook.response;
using ProjectBook.Response;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectBook.Controllers.Backend
{
    [Route("api/auth/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductsController(ApiDbContext dbContext, CloudinaryService cloudinaryService) : ControllerBase
    {
        private readonly ApiDbContext _dbContext = dbContext;
        private readonly CloudinaryService _cloudinaryService = cloudinaryService;
        // GET: api/<ProductsController>
        [HttpGet("page/{pageNumber}")]
        public IActionResult Get(string sort, int pageNumber = 1, string keyword = null)
        {
            IQueryable<Product> products;
            switch (sort)
            {
                case "desc":
                    products = _dbContext.Products.OrderByDescending(u => u.Id).Include(p => p.ProductImages).Include(p => p.ProductDetails);
                    break;
                case "asc":
                    products = _dbContext.Products.OrderBy(u => u.Id).Include(p => p.ProductImages).Include(p => p.ProductDetails);
                    break;
                default:
                    products = _dbContext.Products.Include(p => p.ProductImages).Include(p => p.ProductDetails);
                    break;
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                products = products.Where(u => u.Name.Contains(keyword) || u.ShortDescription.Contains(keyword) || u.FullDescription.Contains(keyword));
            }

            var currentPageNumber = pageNumber;
            var currentPageSize = 2;
            int totalItems = products.Count();

            int startCount = (pageNumber - 1) * currentPageSize + 1;
            int endCount = startCount + currentPageSize - 1;

            int totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

            // Kiểm tra giá trị trang hợp lệ
            currentPageNumber = Math.Max(1, Math.Min(currentPageNumber, totalPages));

            var pagedUsers = products.Skip((currentPageNumber - 1) * currentPageSize)
                          .Take(currentPageSize)
                          .ToList();



            var pageResponse = new ProductPageResponse();
            pageResponse.Total_pages = totalPages;
            pageResponse.Total_items = totalItems;
            pageResponse.Current_page = currentPageNumber;
            pageResponse.Start_count = startCount;
            pageResponse.End_count = endCount;
            pageResponse.Products = pagedUsers;
            return Ok(pageResponse);
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var currentProducts = _dbContext.Products.Include(p => p.ProductImages).Include(p => p.ProductDetails).FirstOrDefault(p => p.Id == id);
            if (currentProducts == null)
            {
                return NotFound();
            }
            return Ok(currentProducts);
        }

        // POST api/<ProductsController>
        [HttpPost]
        public IActionResult Post([FromForm] string data, IFormFile file, IFormFile[] extraFile)
        {
            //convert json -> object
            var productRequest = JsonConvert.DeserializeObject<Product>(data);

            var currentProduct = _dbContext.Products.FirstOrDefault(p => p.Name == productRequest.Name);
            if (currentProduct != null)
            {
                return BadRequest(productRequest.Name);
            }


            productRequest.CreatedAt = DateTime.Now;
            productRequest.UpdatedAt = DateTime.Now;

            if (productRequest.AuthorId > 0)
            {
                var author = _dbContext.Author.Find(productRequest.AuthorId);
                if (author == null)
                {
                    return BadRequest("Author not Found");
                }
                productRequest.Author = author;
            }



            if (productRequest.CategoryId > 0)
            {
                var category = _dbContext.Categories.Find(productRequest.CategoryId);
                if (category == null)
                {
                    return BadRequest("Category Not Found");
                }
                productRequest.Category = category;
            }

            // Product Details

            // CLOUDINARY
            if (file != null)
            {
                var uploadResult = _cloudinaryService.SaveImage(file);
                if (uploadResult.Error != null)
                {
                    return BadRequest("Lỗi khi upload hình ảnh");

                }
                productRequest.MainImage = uploadResult.SecureUrl.ToString();
            }

            // Product Images
            if (extraFile != null && extraFile.Length > 0)
            {

                foreach (var f in extraFile)
                {

                    var uploadResult = _cloudinaryService.SaveImage(f);
                    if (uploadResult.Error != null)
                    {
                        return BadRequest("Lỗi khi upload hình ảnh");

                    }
                    productRequest.addProductImage(uploadResult.SecureUrl.ToString());
                }
            }

            _dbContext.Products.Add(productRequest);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, [FromForm] string data, IFormFile file, IFormFile[] extraFile)
        {
            var currentProduct = _dbContext.Products.Find(id);
            if (currentProduct == null)
            {
                return NotFound();
            }

            //convert json -> object
            var productRequest = JsonConvert.DeserializeObject<Product>(data);

            if (productRequest.Name != currentProduct.Name)
            {
                var checkName = _dbContext.Products.FirstOrDefault(p => p.Name == productRequest.Name);
                if (checkName != null)
                {
                    return BadRequest(checkName.Name);
                }
            }

            currentProduct.Name = productRequest.Name;
            currentProduct.Alias = productRequest.Alias;
            currentProduct.ShortDescription = productRequest.ShortDescription;
            currentProduct.FullDescription = productRequest.FullDescription;
            currentProduct.Cost = productRequest.Cost;
            currentProduct.Price = productRequest.Price;
            currentProduct.Sale = productRequest.Sale;
            currentProduct.Enabled = productRequest.Enabled;

            currentProduct.UpdatedAt = DateTime.Now;

            var author = _dbContext.Author.Find(productRequest.AuthorId);
            if (author == null)
            {
                return BadRequest("Author not Found");
            }
            currentProduct.Author = author;


            var category = _dbContext.Categories.Find(productRequest.CategoryId);
            if (category == null)
            {
                return BadRequest("Category Not Found");
            }
            currentProduct.Category = category;

            // Product Details
            List<string> removeDetail = new List<string>();
            foreach (var detail in productRequest.ProductDetails.ToList())
            {
                removeDetail.Add(detail.Name);
                var detailInDB = _dbContext.ProductDetails.FirstOrDefault(d => d.Name == detail.Name && d.ProductId == currentProduct.Id);
                if (detailInDB != null)
                {
                    detailInDB.Value = detail.Value;
                }
                else
                {
                    currentProduct.addProductDetails(detail.Name, detail.Value);
                }
            }

            var detailsInDB = _dbContext.ProductDetails.Where(d => d.ProductId == currentProduct.Id);
            foreach (var detailInDB in detailsInDB)
            {
                if (!removeDetail.Contains(detailInDB.Name))
                {
                    _dbContext.ProductDetails.Remove(detailInDB);
                }

            }

            // CLOUDINARY
            if (file != null)
            {
                _cloudinaryService.deleteImage(currentProduct.MainImage);
                var uploadResult = _cloudinaryService.SaveImage(file);
                if (uploadResult.Error != null)
                {
                    return BadRequest("Lỗi khi upload hình ảnh");

                }
                currentProduct.MainImage = uploadResult.SecureUrl.ToString();
            }

            // Product Images
            if (extraFile != null && extraFile.Length > 0)
            {
                var imageInDB = _dbContext.ProductImages.Where(i => i.ProductId == currentProduct.Id);
                foreach (var image in imageInDB)
                {
                    _cloudinaryService.deleteImage(image.Name);
                    _dbContext.ProductImages.Remove(image);
                }
                foreach (var f in extraFile)
                {
                    var uploadResult = _cloudinaryService.SaveImage(f);
                    if (uploadResult.Error != null)
                    {
                        return BadRequest("Lỗi khi upload hình ảnh");

                    }
                    currentProduct.addProductImage(uploadResult.SecureUrl.ToString());
                }
            }



            _dbContext.SaveChanges();
            return Ok(currentProduct);

        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var currentProducts = _dbContext.Products.Find(id);
            if (currentProducts == null)
            {
                return NotFound();
            }
            _cloudinaryService.deleteImage(currentProducts.MainImage);

            var listImages = _dbContext.ProductImages.Where(p => p.ProductId == id).ToList();
            foreach (var image in listImages)
            {
                _cloudinaryService.deleteImage(image.Name);
            }

            _dbContext.Products.Remove(currentProducts);
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("authors")]
        public IActionResult GetAuthor()
        {
            var listAuthors = _dbContext.Author;
            List<AuthorDTO> authorsDTO = new List<AuthorDTO>();
            foreach (var author in listAuthors)
            {
                authorsDTO.Add(new AuthorDTO(author.Id, author.Name));
            }
            return Ok(authorsDTO);
        }

        [HttpGet]
        [Route("categories")]
        public IActionResult GetCategories()
        {
            var listCategories = _dbContext.Categories;
            List<CategoryDTO> categoriesDTO = new List<CategoryDTO>();
            foreach (var cate in listCategories)
            {
                categoriesDTO.Add(new CategoryDTO(cate.Id, cate.Name));
            }
            return Ok(categoriesDTO);
        }
    }
}
