using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBook.Data;
using ProjectBook.Response;
using ProjectBook.Serivces;

namespace ProjectBook.Controllers.Frontend
{
    [Route("api/auth/[controller]")]
    [Authorize]
    [ApiController]
    public class CartController(ApiDbContext dbContext) : ControllerBase
    {
        private readonly ApiDbContext _dbContext = dbContext;
        private readonly CartService cartService = new CartService(dbContext);

        [HttpPost("add/{productId}/{customerId}/{quantity}")]
        public IActionResult addProductToCart(int productId, int customerId, int quantity)
        {
            if (customerId == 0)
            {
                return NotFound();
            }
            if (quantity <= 0)
            {
                return NotFound();
            }
            cartService.addProduct(productId, quantity, customerId);
            return Ok();
        }

        [HttpGet("{cusomnerId}")]
        public IActionResult GetCart(int cusomnerId)
        {
            var listCart = _dbContext.Cart_items.Include(c => c.Product).Include(c => c.Customer).Where(c => c.CustomerId == cusomnerId);

            List<CartResponse> cartResponse = new List<CartResponse>();
            foreach (var item in listCart)
            {
                var cart = new CartResponse();
                cart.Id = item.Id;
                cart.Quantity = item.Quantity;

                var product = new CartProductResponse();
                product.Id = item.Product.Id;
                product.Name = item.Product.Name;
                product.Price = item.Product.Price;
                product.MainImage = item.Product.MainImage;

                cart.Product = product;

                cartResponse.Add(cart);
            }
            return Ok(cartResponse);
        }

        [HttpDelete("remove/{productId}/{customerId}")]
        public IActionResult DeleteProductInCart(int productId,int customerId)
        {
            var cart = _dbContext.Cart_items.FirstOrDefault(c => c.ProductId == productId && c.CustomerId == customerId);
            if (cart == null)
            {
                return NotFound();
            }
            _dbContext.Cart_items.Remove(cart);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
