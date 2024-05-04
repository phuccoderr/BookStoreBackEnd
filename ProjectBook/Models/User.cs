using System.ComponentModel.DataAnnotations;

namespace ProjectBook.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Email cannot be null!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Enabled { get; set; }
        public string Photo { get; set; }

        public string Role { get; set; }
       
    }

}
