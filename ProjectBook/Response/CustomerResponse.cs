namespace ProjectBook.Response
{
    public class CustomerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string Photo { get; set; }
        public DateTime Created_Time { get; set; }
        public bool Enabled { get; set; }
    }
}
