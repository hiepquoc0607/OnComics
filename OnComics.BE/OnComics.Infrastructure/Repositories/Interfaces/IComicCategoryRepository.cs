using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IComicCategoryRepository : IGenericRepository<Comiccategory>
    {
        Task DeleteComicCateoriesAsync(Guid comicId);
    }
}
