namespace ProjectBook.DTO
{
    public class AccountFacebook
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public Picture Picture { get; set; }
    }

    public class Picture
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public int Height { get; set; }
        public bool Is_silhouette { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
    }
}
