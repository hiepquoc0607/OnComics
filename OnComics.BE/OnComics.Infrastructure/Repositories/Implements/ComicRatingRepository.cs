using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Domains;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class ComicRatingRepository : GenericRepository<Comicrating>, IComicRatingRepository
    {
        public ComicRatingRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get Rating By Account Id And Comic Id
        public async Task<Comicrating?> GetRatingByAccIdAndComicIdAsync(int accId, int comicId)
        {
            return await _context.Comicratings
                        .AsNoTracking()
                        .Where(r => r.AccountId == accId &&
                            r.ComicId == comicId)
                        .FirstOrDefaultAsync();
        }

        //Count Rating Record By Account Id
        public async Task<int> CountRatingByAccountIdAsync(int id)
        {
            return await _context.Comicratings
                .AsNoTracking()
                .Where(r => r.AccountId == id)
                .CountAsync();
        }

        //Count Rating Record By Comic Id
        public async Task<int> CountRatingByComicIdAsync(int id)
        {
            return await _context.Comicratings
                .AsNoTracking()
                .Where(r => r.ComicId == id)
                .CountAsync();
        }
    }
}
