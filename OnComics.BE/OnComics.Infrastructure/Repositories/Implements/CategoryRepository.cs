using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Domains;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Check If Category Is Existed
        public async Task<bool> CheckCategoryIsExistedAsync(string name)
        {
            return await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Name.Equals(name));
        }

        //Get Category Name Array
        public async Task<string[]> GetCateNamesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Select(c => c.Name)
                .ToArrayAsync();
        }
    }
}
