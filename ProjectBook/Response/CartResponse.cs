namespace ProjectBook.Response
{
    public class CartResponse
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public CartProductResponse Product { get; set; }

        public CartResponse() {
        }
    }
}
