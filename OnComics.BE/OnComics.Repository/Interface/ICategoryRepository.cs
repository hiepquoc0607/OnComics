using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Category;
using OnComics.Library.Models.Response.Api;

namespace OnComics.Repository.Interface
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<(IEnumerable<Category>?, Pagination)> GetCategoriesAsync(GetCategoryReq getCategoryReq);

        Task<Category?> GetCategoryByIdAsync(int id, bool isTracking);

        Task CreateCategoryAsync(Category category);

        Task CreateCategoriesAsync(IEnumerable<Category> categories);

        Task UpdateCategoryAsync(Category category);

        Task DeleteCategoryAsync(int id);

        Task<bool> CheckCategoryIsExistedAsync(string name);

        Task<string[]> GetCateNamesAsync();
    }
}