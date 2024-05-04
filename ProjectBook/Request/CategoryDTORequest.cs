using System.ComponentModel.DataAnnotations;

namespace ProjectBook.Request
{
    public class CategoryDTORequest
    {

        [Required(ErrorMessage = "Category name can't be null or empty!")]
        public string Name { get; set; }

        public string Alias { get; set; }
        public string ImageURL { get; set; }
        public IFormFile File { get; set; }
    }
}
