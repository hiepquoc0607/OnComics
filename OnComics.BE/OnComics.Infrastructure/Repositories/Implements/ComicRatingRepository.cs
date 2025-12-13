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
        public async Task<RatingsInfo> GetRatingsAsync(
            Expression<Func<Comicrating, bool>>? filter = null,
            Func<IQueryable<Comicrating>, IOrderedQueryable<Comicrating>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            try
            {
                var query = _context.Comicratings
                    .AsNoTracking()
                    .AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                if (query.ToListAsync() == null)
                    return new RatingsInfo(null, null, null);

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

                return new RatingsInfo(ratings, accounts, comics);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get Rating By Account Id And Comic Id
        public async Task<Comicrating?> GetRatingByAccIdAndComicIdAsync(Guid accId, Guid comicId)
        {
            try
            {
                return await _context.Comicratings
                    .AsNoTracking()
                    .Where(r => r.AccountId == accId &&
                        r.ComicId == comicId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Count Rating Record
        public async Task<int> CountRatingAsync(Guid id, bool isComicId)
        {
            try
            {
                switch (isComicId)
                {
                    case true:
                        return await _context.Comicratings
                            .AsNoTracking()
                            .Where(r => r.ComicId == id)
                            .CountAsync();
                    case false:
                        return await _context.Comicratings
                            .AsNoTracking()
                            .Where(r => r.AccountId == id)
                            .CountAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Caculate Comic Average Rating
        public async Task<decimal> AverageRatingAsync(Guid comicId, double? newRating, decimal? oldRating)
        {
            try
            {
                int rateNo = await _context.Comicratings
                    .AsNoTracking()
                    .Where(r => r.ComicId == comicId)
                    .CountAsync();

                decimal average = _context.Comicratings
                    .AsNoTracking()
                    .Where(r => r.ComicId == comicId)
                    .Select(r => r.Rating)
                    .AverageAsync()
                    .Result;

                decimal result = 0;

                if (oldRating.HasValue && newRating.HasValue)
                {
                    result = (average * (decimal)rateNo - oldRating.Value + (decimal)newRating) / rateNo;
                }
                else if (oldRating.HasValue && !newRating.HasValue)
                {
                    result = (average * (decimal)rateNo - oldRating.Value) / (rateNo - 1);
                }
                else if (!oldRating.HasValue && newRating.HasValue)
                {
                    result = (average * rateNo + (decimal)newRating) / (rateNo + 1);
                }
                else
                {
                    result = average;
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
