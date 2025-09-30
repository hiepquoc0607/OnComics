using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Domains;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class ComicRatingRepository : GenericRepository<Comicrating>, IComicRatingRepository
    {
        public ComicRatingRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get All Ratings By Account Id
        public async Task<(IEnumerable<Comicrating>, Dictionary<int, string>)> GetRatingsByAccountId(
            Expression<Func<Comicrating, bool>>? filter = null,
            Func<IQueryable<Comicrating>, IOrderedQueryable<Comicrating>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            var query = _context.Comicratings
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (pageNumber.HasValue && pageSize.HasValue)
                query = query.Skip((pageNumber.Value - 1) * pageSize.Value)
                             .Take(pageSize.Value);

            var projected = await query
                .Select(r => new
                {
                    Rating = r,
                    ComicId = r.Comic.Id,
                    ComicName = r.Comic.Name
                })
                .ToListAsync();

            var ratings = projected.Select(r => r.Rating).ToList();

            var comics = projected.ToDictionary(c => c.ComicId, c => c.ComicName);

            return (ratings, comics);
        }

        //Get All Ratings By Comic id
        public async Task<(IEnumerable<Comicrating>, Dictionary<int, string>)> GetRatingsByComicId(
            Expression<Func<Comicrating, bool>>? filter = null,
            Func<IQueryable<Comicrating>, IOrderedQueryable<Comicrating>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            var query = _context.Comicratings
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (pageNumber.HasValue && pageSize.HasValue)
                query = query.Skip((pageNumber.Value - 1) * pageSize.Value)
                             .Take(pageSize.Value);

            var projected = await query
                .Select(r => new
                {
                    Rating = r,
                    AccId = r.Account.Id,
                    Fullname = r.Account.Fullname
                })
                .ToListAsync();

            var ratings = projected.Select(r => r.Rating).ToList();

            var accounts = projected.ToDictionary(a => a.AccId, a => a.Fullname);

            return (ratings, accounts);
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
