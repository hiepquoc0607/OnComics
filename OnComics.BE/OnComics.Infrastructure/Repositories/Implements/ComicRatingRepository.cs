using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
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

        //Get All Ratings
        public async Task<(IEnumerable<Comicrating>, IDictionary<int, string>, IDictionary<int, string>)> GetRatingsAsync(
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
                    AccountId = r.Account.Id,
                    Fullname = r.Account.Fullname,
                    ComicId = r.Comic.Id,
                    ComicName = r.Comic.Name
                })
                .ToListAsync();

            var ratings = projected.Select(r => r.Rating).ToList();

            var accounts = projected.ToDictionary(a => a.AccountId, a => a.Fullname);

            var comics = projected.ToDictionary(c => c.ComicId, c => c.ComicName);

            return (ratings, accounts, comics);
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

        //Count Rating Record
        public async Task<int> CountRatingAsync(int id, bool isComicId = false)
        {
            switch (isComicId)
            {
                case true:
                    return await _context.Comicratings
                        .AsNoTracking()
                        .Where(r => r.ComicId == id)
                        .CountAsync();
                default:
                    return await _context.Comicratings
                        .AsNoTracking()
                        .Where(r => r.AccountId == id)
                        .CountAsync();
            }
        }
    }
}
