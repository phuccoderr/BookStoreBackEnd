using ProjectBook.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectBook.Request
{
    public class ProductDTORequest
    {
        [Required(ErrorMessage = "name cannot be null!")]
        public string Name { get; set; }
        public string Alias { get; set; }

        [JsonPropertyName("short_description")]
        public string ShortDescription { get; set; }
        [JsonPropertyName("full_description")]
        public string Full_Description { get; set; }
        public float Cost { get; set; }
        public float Price { get; set; }
        public float Sale { get; set; }
        public bool Enabled { get; set; }

        public int AuthorId { get; set; }

        public int CategoryId { get; set; }

        public ICollection<ProductDetails> ProductDetails { get; set; }

    }
}
