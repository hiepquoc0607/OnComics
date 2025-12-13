using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
    {
        public FavoriteRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get All Favorite
        public async Task<FavoritesInfo> GetFavoritesAsync(
            Expression<Func<Favorite, bool>>? filter = null,
            Func<IQueryable<Favorite>, IOrderedQueryable<Favorite>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            try
            {
                var query = _context.Favorites
                    .AsNoTracking()
                    .AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                if (query.ToListAsync() == null)
                    return new FavoritesInfo(null, null, null);

                if (orderBy != null)
                    query = orderBy(query);

                if (pageNumber.HasValue && pageSize.HasValue)
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value)
                                 .Take(pageSize.Value);

                var projected = await query
                    .Select(i => new
                    {
                        Favorites = i,
                        FavoriteId = i.Id,
                        AccountId = i.Account.Id,
                        Fullname = i.Account.Fullname,
                        ComicId = i.Comic.Id,
                        ComicName = i.Comic.Name
                    })
                    .ToListAsync();

                var favorites = projected.Select(i => i.Favorites).ToList();

                var accounts = projected
                    .GroupBy(a => a.FavoriteId)
                    .ToDictionary(
                        a => a.Key,
                        a => (a.First().AccountId, a.First().Fullname));

                var comics = projected
                    .GroupBy(c => c.FavoriteId)
                    .ToDictionary(
                        c => c.Key,
                        c => (c.First().ComicId, c.First().ComicName));

                return new FavoritesInfo(favorites, accounts, comics);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Check If Favorite Is Existed
        public async Task<bool> CheckFavoriteExistedAsync(Guid accId, Guid comicId)
        {
            try
            {
                return await _context.Favorites
                    .AsNoTracking()
                    .AnyAsync(f =>
                        f.AccountId == accId &&
                        f.ComicId == comicId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Count Favorite Record
        public async Task<int> CountFavoriteAsync(Guid id, bool isComic)
        {
            try
            {
                if (isComic == true)
                {
                    return await _context.Favorites
                        .AsNoTracking()
                        .CountAsync(f => f.ComicId == id);
                }
                else
                {
                    return await _context.Favorites
                        .AsNoTracking()
                        .CountAsync(f => f.AccountId == id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
