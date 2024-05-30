using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBook.Data;
using ProjectBook.Helpers;
using ProjectBook.Models;
using ProjectBook.Response;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectBook.Controllers.Backend
{
    [Route("api/auth/[controller]")]
    [ApiController]
    public class OrdersController(ApiDbContext dbContext) : ControllerBase
    {
        private readonly ApiDbContext _dbContext = dbContext;
        // GET: api/<OrdersController>
        [HttpGet("page/{pageNumber}")]
        public IActionResult Get(string sort, int pageNumber = 1, string keyword = null)
        {
            IQueryable<Order> orders;
            switch (sort)
            {
                case "desc":
                    orders = _dbContext.Orders.OrderByDescending(o => o.Oder_time);
                    break;
                case "asc":
                    orders = _dbContext.Orders.OrderBy(u => u.Oder_time);
                    break;
                default:
                    orders = _dbContext.Orders.OrderBy(u => u.Oder_time);
                    break;
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                orders = orders.Where(o => o.Customer.Name.Contains(keyword));
            }

            var currentPageNumber = pageNumber;
            var currentPageSize = 4;
            int totalItems = orders.Count();

            int startCount = (pageNumber - 1) * currentPageSize + 1;
            int endCount = startCount + currentPageSize - 1;

            int totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

            // Kiểm tra giá trị trang hợp lệ
            currentPageNumber = Math.Max(1, Math.Min(currentPageNumber, totalPages));

            var pagedOrders = orders.Skip((currentPageNumber - 1) * currentPageSize)
                          .Take(currentPageSize)
                          .ToList();



            var pageResponse = new OrderPageResponse();
            pageResponse.Total_pages = totalPages;
            pageResponse.Total_items = totalItems;
            pageResponse.Current_page = currentPageNumber;
            pageResponse.Start_count = startCount;
            pageResponse.End_count = endCount;
            pageResponse.Orders = pagedOrders;
            return Ok(pageResponse);
        }


        // DELETE api/<OrdersController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var currentOrder = _dbContext.Orders.Find(id);
            if (currentOrder == null)
            {
                return NotFound();
            }
            _dbContext.Orders.Remove(currentOrder);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
