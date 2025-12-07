using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get All Categories
        public async Task<IEnumerable<Category>?> GetCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .ToListAsync();
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

        //Get Category Id Array
        public async Task<Guid[]> GetCateIdsAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Select(c => c.Id)
                .ToArrayAsync();
        }
    }
}
