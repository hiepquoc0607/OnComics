using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Domains;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class ComicRepository : GenericRepository<Comic>, IComicRepository
    {
        public ComicRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get Comic Names By Each Comic Id
        public async Task<Dictionary<int, string>> GetNamesByIdsAsync(int[] ids)
        {
            return await _context.Comics
                .AsNoTracking()
                .Where(c => ids.Contains(c.Id))
                .ToDictionaryAsync(
                    c => c.Id,
                    c => c.Name);
        }

        //Check If Comic Is Existed
        public async Task<bool> CheckComicExistedAsync(string name, string author)
        {
            return await _context.Comics
                .AsNoTracking()
                .AnyAsync(c =>
                    EF.Functions.Like(c.Name, $"%{name}%") &&
                    EF.Functions.Like(c.Author, $"%{author}%"));
        }

        //Check If Comic Is Existed By Id
        public async Task<bool> CheckComicIdAsync(int id)
        {
            return await _context.Comics
                .AsNoTracking()
                .AnyAsync(c => c.Id == id);
        }

        //Get All Comic Ids
        public async Task<int[]> GetComicIdsAsync()
        {
            return await _context.Comics
                .AsNoTracking()
                .Select(c => c.Id)
                .ToArrayAsync();
        }
    }
}
