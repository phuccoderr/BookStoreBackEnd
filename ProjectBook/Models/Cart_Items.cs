namespace ProjectBook.Models
{
    public class Cart_items
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int Quantity { get; set; }

        public Cart_items() { }
    }
}
