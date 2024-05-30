using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectBook.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "name cannot be null!")]
        public string Name { get; set; }
        public string Alias { get; set; }
        public string MainImage { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public float Quantity { get; set; }
        public float Cost { get; set; }
        public float Price { get; set; }
        public float Sale {  get; set; }
        public bool Enabled { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int AuthorId { get; set; }
        [JsonIgnore]
        public Author Author { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<ProductImages> ProductImages { get; set; }

        public ICollection<ProductDetails> ProductDetails { get; set; }

        public Product()
        {
            ProductDetails = new List<ProductDetails>();
            ProductImages = new List<ProductImages>();
        }

        public void addProductImage(string image)
        {
            this.ProductImages.Add(new ProductImages(image, this));
        }

        public void addProductDetails(string  detailName,string detailValue)
        {
            this.ProductDetails.Add(new ProductDetails(detailName, detailValue,this));
        }

        public float GetDiscount()
        {
            return Price - Price * (Sale / 100);
        }

    }
}
