namespace ProjectBook.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Created_Time { get; set; }
        public bool Enabled { get; set; }
        public string Verification_Code { get; set; }
        public string Veset_Password_Token { get; set; }


    }
}
