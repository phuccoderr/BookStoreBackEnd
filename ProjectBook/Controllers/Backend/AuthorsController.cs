using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectBook.Data;
using ProjectBook.Helpers;
using ProjectBook.Models;
using ProjectBook.response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectBook.Controllers.Backend
{
    [Route("api/auth/[controller]")]
    [Authorize]
    [ApiController]
    public class AuthorsController(ApiDbContext dbContext) : ControllerBase
    {
        private readonly ApiDbContext _dbContext = dbContext;


        [HttpGet("page/{pageNumber}")]
        public IActionResult Get(string sort, int pageNumber = 1, string keyword = null)
        {
            IQueryable<Author> authors;
            switch (sort)
            {
                case "desc":
                    authors = _dbContext.Author.OrderByDescending(u => u.Id);
                    break;
                case "asc":
                    authors = _dbContext.Author.OrderBy(u => u.Id);
                    break;
                default:
                    authors = _dbContext.Author;
                    break;
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                authors = authors.Where(u => u.Name.Contains(keyword));
            }

            var currentPageNumber = pageNumber;
            var currentPageSize = 2;
            int totalItems = authors.Count();

            int startCount = (pageNumber - 1) * currentPageSize + 1;
            int endCount = startCount + currentPageSize - 1;

            int totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

            // Kiểm tra giá trị trang hợp lệ
            currentPageNumber = Math.Max(1, Math.Min(currentPageNumber, totalPages));

            var pagedAuthors = authors.Skip((currentPageNumber - 1) * currentPageSize)
                          .Take(currentPageSize)
                          .ToList();

            var pageResponse = new AuthorPageResponse();
            pageResponse.Authors = pagedAuthors.ToList();
            pageResponse.Total_pages = totalPages;
            pageResponse.Total_items = totalItems;
            pageResponse.Current_page = currentPageNumber;
            pageResponse.Start_count = startCount;
            pageResponse.End_count = endCount;
            return Ok(pageResponse);
        }

        // GET api/<AuthorsController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var currentAuthor = _dbContext.Author.Find(id);
            if (currentAuthor == null)
            {
                return NotFound();
            }

            return Ok(currentAuthor);
        }

        // POST api/<AuthorsController>
        [HttpPost]
        public IActionResult Post([FromBody] Author author)
        {
            var currentAuthor = _dbContext.Author.FirstOrDefault(a => a.Name == author.Name);
            if (currentAuthor != null)
            {
                return BadRequest(author.Name);
            }
            _dbContext.Author.Add(author);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        // PUT api/<AuthorsController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Author author)
        {

            var currentAuthor = _dbContext.Author.Find(id);
            if (currentAuthor == null)
            {
                return NotFound();
            }
            currentAuthor.Name = author.Name;
            currentAuthor.DateOfBirth = author.DateOfBirth;
            _dbContext.SaveChanges();
            return Ok(currentAuthor);
        }

        // DELETE api/<AuthorsController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var currentAuthor = _dbContext.Author.Find(id);
            if (currentAuthor == null)
            {
                return NotFound();
            }
            _dbContext.Remove(currentAuthor);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
