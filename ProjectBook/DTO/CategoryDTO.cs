namespace ProjectBook.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CategoryDTO() { }
        public CategoryDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
