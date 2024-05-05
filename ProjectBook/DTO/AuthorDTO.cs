namespace ProjectBook.DTO
{
    public class AuthorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public AuthorDTO() { }
        public AuthorDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
