using OnComics.Library.Models.Data;
using OnComics.Repository.Interface;

namespace OnComics.Repository.Implement
{
    public class ComicCategoryRepository : GenericRepository<Comiccategory>, IComicCategoryRepository
    {
        public ComicCategoryRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Create Comic Category
        public async Task CreateComicCateAsync(Comiccategory comiccategory)
        {
            await InsertAsync(comiccategory);
        }

        //Update Comic Category
        public async Task UpdateComicCateAsync(Comiccategory comiccategory)
        {
            await UpdateAsync(comiccategory);
        }

        //Delete Comic Category
        public async Task DeleteComicCateAsync(int id)
        {
            await DeleteAsync(id);
        }

        //Bulk (Range) Delete Comic Category
        public async Task BulkDeleteComicCateAsync(IEnumerable<Comiccategory> comiccategories)
        {
            await DeleteRangeAsync(comiccategories);
        }
    }
}