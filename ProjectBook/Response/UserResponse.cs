namespace ProjectBook.response
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Photo {  get; set; }

        public bool Enabled { get; set; }

        public string Role { get; set; }
    }
}
