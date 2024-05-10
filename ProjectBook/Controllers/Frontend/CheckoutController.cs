using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBook.Data;
using ProjectBook.Models;
using ProjectBook.Serivces;

namespace ProjectBook.Controllers.Frontend
{
    [Route("api/auth/[controller]")]
    [ApiController]
    public class CheckoutController(ApiDbContext dbContext) : ControllerBase
    {
        private readonly ApiDbContext _dbContext = dbContext;
        private CartService _cartService = new CartService(dbContext);

        [HttpPost("{customerId}")]
        public IActionResult PlaceOrder(int customerId, [FromBody] CheckoutInfo info)
        {
            var listCart = _dbContext.Cart_items.Include(c => c.Product).Where(c => c.CustomerId == customerId);
            var customer = _dbContext.Customers.Find(customerId);
            Order order = _cartService.Checkout(listCart.ToList(), info, customer);

            foreach(var item in listCart)
            {
                _dbContext.Cart_items.Remove(item);
            }

           _dbContext.SaveChanges();


            return Ok(order);

        }

        
    }
}
