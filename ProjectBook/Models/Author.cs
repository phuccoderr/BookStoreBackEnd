

using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

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

        public Author ()
        {

        }

        public static Author CopyIdAndName(int id,string name )
        {
            var author = new Author();
            author.Id = id;
            author.Name = name;
            return author;
        }
    }
}
