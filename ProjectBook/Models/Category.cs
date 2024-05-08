using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectBook.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name can't be null or empty!")]
        public string Name { get; set; }

        public string Alias { get; set; }
        public string ImageURL { get; set; }

        [JsonIgnore]
        public ICollection<Product> Products { get; set; }

        public Category()
        {

        }

        public static Category CopyIdAndName(int id ,string name)
        {
            Category copy = new Category();
            copy.Id = id;
            copy.Name = name;
            return copy;
        }

    }
}
