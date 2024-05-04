using System.ComponentModel.DataAnnotations;

namespace ProjectBook.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name can't be null or empty!")]
        public string Name { get; set; }

        public string Alias { get; set; }
        public string ImageURL { get; set; }

        public ICollection<Product> Products { get; set; }

    }
}
