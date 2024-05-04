using System.Text.Json.Serialization;

namespace ProjectBook.Models
{
    public class ProductImages
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int ProductId { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }

        public ProductImages()
        {

        }

        public ProductImages(string name, Product product) {
            this.Name = name;
            this.Product = product;
        }
    }
}
