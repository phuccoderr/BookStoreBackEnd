using Microsoft.EntityFrameworkCore;
using ProjectBook.Models;

namespace ProjectBook.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext() { }
        public ApiDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<ProductDetails> ProductDetails { get; set; }
        public DbSet<User> Users { get; set; }  
        public DbSet<Author> Author { get; set; }   
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Cart_items> Cart_items { get; set;}
    }
}
