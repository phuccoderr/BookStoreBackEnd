using System.ComponentModel.DataAnnotations;

namespace ProjectBook.Request
{
    public class AuthRequest
    {

        [Required(ErrorMessage = "Email cannot be null!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Password {  get; set; }
    }
}
