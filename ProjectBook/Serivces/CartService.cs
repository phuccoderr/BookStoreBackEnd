using CloudinaryDotNet.Actions;
using ProjectBook.Data;
using ProjectBook.Models;

namespace ProjectBook.Serivces
{
    public class CartService(ApiDbContext dbContext)
    {
        private readonly ApiDbContext _dbContext = dbContext;

        public void addProduct(int productId, int quantity,int customerId)
        {
            var product = _dbContext.Products.Find(productId);
            var customer = _dbContext.Customers.Find(customerId);

            var cartItem =_dbContext.Cart_items.FirstOrDefault(c => c.ProductId == productId && c.CustomerId == customerId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _dbContext.SaveChanges();
            } else
            {
                cartItem = new Cart_items();
                cartItem.Product = product;
                cartItem.Customer = customer;
                cartItem.Quantity = quantity;
                _dbContext.Cart_items.Add(cartItem);
                _dbContext.SaveChanges();
            }
            
        }

        public Order Checkout(List<Cart_items> cartItems,CheckoutInfo info, Customer customer)
        {
            var order = new Order();

            float sum = 0;
            foreach (var item in cartItems)
            {
                sum += (item.Product.Price * item.Quantity);
                OrderDetails detail = new OrderDetails();
                detail.Product = item.Product;
                detail.Quantity = item.Quantity;
                detail.Total = item.Product.Price * item.Quantity;
                detail.Product_cost = item.Product.Price;
                order.Details.Add(detail);
                var currentProduct = _dbContext.Products.FirstOrDefault(p => p.Id == item.Product.Id);
                currentProduct.Quantity = currentProduct.Quantity - item.Quantity;
            }

            order.Customer = customer;
            order.Address = info.Address;
            order.Name = info.Name;
            order.Phone_number = info.Phone_number;
            order.Payment = info.Payment;
            order.Oder_time = DateTime.Now;
            order.Total = sum;

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            return order;
        }
    }
}
