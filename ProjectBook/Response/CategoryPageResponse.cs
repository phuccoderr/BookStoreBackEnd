using ProjectBook.Models;
using ProjectBook.response;

namespace ProjectBook.Response
{
    public class CategoryPageResponse : IPageResponse
    {
        public int Total_items { get; set; }
        public int Total_pages { get; set; }
        public int Current_page { get; set; }
        public int Start_count { get; set; }
        public int End_count { get; set; }
        public List<Category> Categories { get; set; }
    }
}
