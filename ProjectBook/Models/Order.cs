using System.Text.Json.Serialization;

namespace ProjectBook.Models
{
    public class Order
    {
        public int Id { get; set; }
        public float Total { get; set; }
        public DateTime Oder_time { get; set; }
        public string Payment {  get; set; }

        public string Address { get; set; }
        public string Name {  get; set; }
        public string Phone_number { get; set; }

        public int CustomerId {  get; set; }
        [JsonIgnore]
        public Customer Customer { get; set; }

        public ICollection<OrderDetails> Details { get; set; }

        public Order()
        {
            Details = new List<OrderDetails>();
        }

    }
}
