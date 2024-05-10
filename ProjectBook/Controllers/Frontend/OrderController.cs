using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBook.Data;

namespace ProjectBook.Controllers.Frontend
{
    [Route("api/auth/[controller]")]
    [ApiController]
    
    public class OrderController(ApiDbContext dbContext) : ControllerBase
    {
        private readonly ApiDbContext _dbContext =dbContext;
        [HttpGet("{customerId}")]
        public IActionResult GetOrder(int customerId)
        {
            var currentOrder = _dbContext.Orders.Include(o => o.Details).Where(o => o.CustomerId == customerId);
            return Ok(currentOrder);

        }

    }
}
