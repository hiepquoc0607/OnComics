using OnComics.Library.Models.Data;

namespace OnComics.Repository.Interface
{
    public interface IComicCategoryRepository
    {
        Task CreateComicCateAsync(Comiccategory comiccategory);

        Task UpdateComicCateAsync(Comiccategory comiccategory);

        Task DeleteComicCateAsync(int id);

        Task BulkDeleteComicCateAsync(IEnumerable<Comiccategory> comiccategories);
    }
}