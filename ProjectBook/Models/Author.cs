using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectBook.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "name cannot be null!")]
        public string Name { get; set; }
        [JsonPropertyName("date_of_birth")]
        public DateTime DateOfBirth { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
