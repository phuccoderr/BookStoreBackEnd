using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectBook.Data;
using ProjectBook.Helpers;
using ProjectBook.Models;
using ProjectBook.Request;
using ProjectBook.response;
using ProjectBook.Response;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace ProjectBook.Controllers.Backend
{
    [Route("api/auth/[controller]")]
    [ApiController]
    /*[Authorize]*/
    public class CategoriesController(ApiDbContext dbContext, CloudinaryService cloudinaryService) : ControllerBase
    {
        private readonly ApiDbContext _dbContext = dbContext;
        private readonly CloudinaryService _cloudinaryService = cloudinaryService;

        [HttpGet("page/{pageNumber}")]
        public IActionResult GetListCategories(string sort, int pageNumber = 1, string keyword = null)
        {
            IQueryable<Category> categories;
            switch (sort)
            {
                case "desc":
                    categories = _dbContext.Categories.OrderByDescending(u => u.Id); // 5 4 3 2 1
                    break;
                case "asc":
                    categories = _dbContext.Categories.OrderBy(u => u.Id); // 1 2 3 4 5
                    break;
                default:
                    categories = _dbContext.Categories;
                    break;
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                categories = categories.Where(c => c.Name.Contains(keyword));
            }

            var currentPageNumber = pageNumber;
            var currentPageSize = 2;
            int totalItems = categories.Count();

            int startCount = (pageNumber - 1) * currentPageSize + 1;
            int endCount = startCount + currentPageSize - 1;

            int totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

            // Kiểm tra giá trị trang hợp lệ
            currentPageNumber = Math.Max(1, Math.Min(currentPageNumber, totalPages));

            var pagedCategories = categories.Skip((currentPageNumber - 1) * currentPageSize)
                          .Take(currentPageSize)
                          .ToList();

            var pageResponse = new CategoryPageResponse();
            pageResponse.Total_pages = totalPages;
            pageResponse.Total_items = totalItems;
            pageResponse.Current_page = currentPageNumber;
            pageResponse.Start_count = startCount;
            pageResponse.End_count = endCount;
            pageResponse.Categories = pagedCategories;
            return Ok(pageResponse);
        }

        [HttpGet("{id}")]
        public IActionResult GetCategory(int id)
        {
            var currentCategory = _dbContext.Categories.Find(id);
            if (currentCategory == null)
            {
                return NotFound();
            }
            return Ok(currentCategory);
        }

        [HttpPost]
        public IActionResult AddCategory([FromForm] CategoryDTORequest categoryRequest)
        {
            var currentCategory = _dbContext.Categories.FirstOrDefault(c => c.Name == categoryRequest.Name);
            if (currentCategory != null)
            {
                return BadRequest(categoryRequest.Name);
            }

            var category = new Category();
            category.Name = categoryRequest.Name;
            category.Alias = categoryRequest.Alias;

            // CLOUDINARY
            if (categoryRequest.File != null)
            {
                var uploadResult = _cloudinaryService.SaveImage(categoryRequest.File);
                if (uploadResult.Error != null)
                {
                    return BadRequest("Lỗi khi upload hình ảnh");

                }
                category.ImageURL = uploadResult.SecureUrl.ToString();
            }

            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}")]
        public IActionResult PutCategory(int id, [FromForm] CategoryDTORequest categoryRequest)
        {
            var currentCategory = _dbContext.Categories.Find(id);
            if (currentCategory == null)
            {
                return NotFound();
            }

            if (categoryRequest.File != null)
            {
                _cloudinaryService.deleteImage(currentCategory.ImageURL);
                var uploadResult = _cloudinaryService.SaveImage(categoryRequest.File);
                if (uploadResult.Error != null)
                {
                    return BadRequest("Lỗi khi upload hình ảnh");

                }
                currentCategory.ImageURL = uploadResult.SecureUrl.ToString();
            }
            currentCategory.Name = categoryRequest.Name;
            currentCategory.Alias = categoryRequest.Alias;
            _dbContext.SaveChanges();
            return Ok(currentCategory);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCateogry(int id)
        {
            var currentCategory = _dbContext.Categories.Find(id);
            if (currentCategory == null)
            {
                return NotFound();
            }
            _cloudinaryService.deleteImage(currentCategory.ImageURL);
            _dbContext.Categories.Remove(currentCategory);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
