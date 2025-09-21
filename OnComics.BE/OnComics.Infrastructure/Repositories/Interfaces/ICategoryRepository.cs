using OnComics.Infrastructure.Domains;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> CheckCategoryIsExistedAsync(string name);

        Task<string[]> GetCateNamesAsync();
    }
}
