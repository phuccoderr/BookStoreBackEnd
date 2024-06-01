using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectBook.Data;
using ProjectBook.Models;
using ProjectBook.response;
using ProjectBook.Response;

namespace ProjectBook.Controllers.Backend
{
    [Route("api/auth/[controller]")]
    [Authorize]
    [ApiController]
    public class CustomersController(ApiDbContext dbContext) : ControllerBase
    {
        private readonly ApiDbContext _dbContext = dbContext;

        [HttpGet("page/{pageNumber}")]
        public IActionResult GetCustomer(string sort, int pageNumber = 1, string keyword = null)
        {
            IQueryable<Customer> customers;
            switch (sort)
            {
                case "desc":
                    customers = _dbContext.Customers.OrderByDescending(u => u.Id);
                    break;
                case "asc":
                    customers = _dbContext.Customers.OrderBy(u => u.Id);
                    break;
                default:
                    customers = _dbContext.Customers;
                    break;
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                customers = customers.Where(u => u.Name.Contains(keyword) || u.Email.Contains(keyword));
            }

            var currentPageNumber = pageNumber;
            var currentPageSize = 2;
            int totalItems = customers.Count();

            int startCount = (pageNumber - 1) * currentPageSize + 1;
            int endCount = startCount + currentPageSize - 1;

            int totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

            // Kiểm tra giá trị trang hợp lệ
            currentPageNumber = Math.Max(1, Math.Min(currentPageNumber, totalPages));

            var pagedCustomers = customers.Skip((currentPageNumber - 1) * currentPageSize)
                          .Take(currentPageSize)
                          .ToList();

            var pageResponse = new CustomerPageResponse();
            pageResponse.Total_pages = totalPages;
            pageResponse.Total_items = totalItems;
            pageResponse.Current_page = currentPageNumber;
            pageResponse.Start_count = startCount;
            pageResponse.End_count = endCount;
            pageResponse.Customers = listEntityToDTOs(pagedCustomers);
            return Ok(pageResponse);
        }

        [HttpGet("{id}/enabled/{enabled}")]
        public IActionResult EnabledCustomer(int id, bool enabled)
        {
            var currentCustomer = _dbContext.Customers.Find(id);
            currentCustomer.Enabled = enabled;
            _dbContext.SaveChanges();
            return Ok();
        }

        private List<CustomerResponse> listEntityToDTOs(List<Customer> customers)
        {
            List<CustomerResponse> dtos = new List<CustomerResponse>();
            foreach (var item in customers)
            {
                dtos.Add(EntityToDTO(item));
            }
            return dtos;
        }

        private CustomerResponse EntityToDTO(Customer customer)
        {
            CustomerResponse customerResponse = new CustomerResponse()
            {
                Email = customer.Email,
                Name = customer.Name,
                Id = customer.Id,
                Photo = customer.Photo,
                Enabled = customer.Enabled,
            };
            return customerResponse;
        }
    }
}
