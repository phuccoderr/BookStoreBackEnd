using System.Text.Json.Serialization;

namespace ProjectBook.Models
{
    public class ProductDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public int ProductId { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }

        public ProductDetails()
        {

        }

        public ProductDetails(string detailName,string detailValue, Product product)
        {
            this.Name = detailName;
            this.Value = detailValue;
            this.Product = product;
        }
    }
}
