using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>?> GetCategoriesAsync();

        Task<bool> CheckCategoryIsExistedAsync(string name);

        Task<string[]> GetCateNamesAsync();

        Task<Guid[]> GetCateIdsAsync();
    }
}
