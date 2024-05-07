
using ProjectBook.Data;
using ProjectBook.Models;
using ProjectBook.Response;

namespace ProjectBook.Serivces
{
    public class ProductService(ApiDbContext dbContext)
    {
        private readonly ApiDbContext _dbContext = dbContext;
        public ProductPageResponse listByCategory(int pageNum,int cateId,string sort)
        {
            IQueryable<Product> products;
            products = _dbContext.Products;
            if (cateId > 0)
            {
                products = _dbContext.Products.Where(p => p.CategoryId == cateId);
            }
           
            switch (sort)
            {
                case "desc":
                    products = products.OrderByDescending(u => u.Id);
                    break;
                case "asc":
                    products = products.OrderBy(u => u.Id);
                    break;
                default:
                    break;
            }
            var currentPageNumber = pageNum;
            var currentPageSize = 16;
            int totalItems = products.Count();

            int startCount = (pageNum - 1) * currentPageSize + 1;
            int endCount = startCount + currentPageSize - 1;

            int totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

            // Kiểm tra giá trị trang hợp lệ
            currentPageNumber = Math.Max(1, Math.Min(currentPageNumber, totalPages));

            var pagedUsers = products.Skip((currentPageNumber - 1) * currentPageSize)
                          .Take(currentPageSize)
                          .ToList();

            var pageResponse = new ProductPageResponse();
            pageResponse.Total_pages = totalPages;
            pageResponse.Total_items = totalItems;
            pageResponse.Current_page = currentPageNumber;
            pageResponse.Start_count = startCount;
            pageResponse.End_count = endCount;
            pageResponse.Products = pagedUsers;
            return pageResponse;

        }
    }
}
