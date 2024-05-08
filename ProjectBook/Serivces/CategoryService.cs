using ProjectBook.Data;
using ProjectBook.DTO;
using ProjectBook.Models;

namespace ProjectBook.Serivces
{
    public class CategoryService(ApiDbContext dbContext)
    {
        private readonly ApiDbContext _dbContext = dbContext;

        public Category getCategoryById(int id)
        {
            var currentCategory = _dbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (currentCategory != null)
            {
                return currentCategory;
            }
            return null;
        }

        public int getCategory (string alias)
        {
            var currentCategory = _dbContext.Categories.FirstOrDefault(c =>  c.Alias == alias);
            if (currentCategory != null)
            {   
                return currentCategory.Id;
            }
            return 0;   
        }

        public List<CategoryDTO> liSTCategories()
        {
            var categories = _dbContext.Categories.ToList();
            List<CategoryDTO> categoryDTOs = new List<CategoryDTO>();
            foreach (var category in categories)
            {
                categoryDTOs.Add(new CategoryDTO(category.Id, category.Name,category.Alias));
            }
            return categoryDTOs;
        }
    }
}
