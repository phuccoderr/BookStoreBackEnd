namespace ProjectBook.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public CategoryDTO() { }
        public CategoryDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public CategoryDTO(int id, string name,string alias)
        {
            Id = id;
            Name = name;
            Alias = alias;
        }
    }
}
