
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ProjectBook.Data;
using ProjectBook.Models;
using ProjectBook.Response;

namespace ProjectBook.Serivces
{
    public class ProductService(ApiDbContext dbContext)
    {
        private readonly ApiDbContext _dbContext = dbContext;
        public ProductPageResponse listByCategory(int pageNum,int cateId,string sort,string keyword)
        {
            IQueryable<Product> products;
           
            if (cateId > 0)
            {
                products = _dbContext.Products.Where(p => p.CategoryId == cateId);
            } else
            {
                products = _dbContext.Products;
            }
           
            switch (sort)
            {
                case "max_price":
                    products = products.OrderByDescending(u => u.Price);
                    break;
                case "min_price":
                    products = products.OrderBy(u => u.Price);
                    break;
                default:
                    
                    break;
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                products = products.Where(u => u.Name.Contains(keyword) || u.ShortDescription.Contains(keyword) || u.FullDescription.Contains(keyword));
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

        public Product getProductByAlias(string alias)
        {
            var currentProduct = _dbContext.Products.Include(p => p.ProductDetails).Include(p => p.ProductImages).FirstOrDefault(p => p.Alias == alias);
            if (currentProduct == null)
            {
                return null;
            }
            return currentProduct;
        }
    }
}
