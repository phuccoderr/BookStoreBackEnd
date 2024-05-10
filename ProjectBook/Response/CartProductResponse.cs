namespace ProjectBook.Response
{
    public class CartProductResponse
    {
        public int Id { get; set; }
        public float Price { get; set; }
        public string Name { get; set; }
        public string MainImage { get; set; }  

        public CartProductResponse() { }
    }
}
