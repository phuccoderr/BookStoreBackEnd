namespace ProjectBook.response
{
    public interface IPageResponse
    {
        public int Total_items { get; set; }
        public int Total_pages { get; set; }
        public int Current_page { get; set; }

        public int Start_count { get; set; }
        public int End_count { get; set; }
    }
}
