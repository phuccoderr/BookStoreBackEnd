using System.Text.Json.Serialization;

namespace ProjectBook.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public float Product_cost { get; set; }
        public int Quantity { get; set; }
        public float Total { get; set; }
        public int OrderId { get; set;}
        [JsonIgnore]
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public OrderDetails ()
        {

        }

        public OrderDetails(int quantity, float productCost, float subtotal)
        {
            this.Quantity = quantity;
            this.Product_cost = productCost;
           this.Total = subtotal;
        }
    }
}
