using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class ComicCategoryRepository : GenericRepository<Comiccategory>, IComicCategoryRepository
    {
        public ComicCategoryRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Delete ComicCategories By Comic Id
        public async Task DeleteComicCateoriesAsync(Guid comicId)
        {
            var records = await _context.Comiccategories
                .AsNoTracking()
                .Where(cc => cc.ComicId == comicId)
                .ToListAsync();

            await BulkDeleteAsync(records);
        }
    }
}
