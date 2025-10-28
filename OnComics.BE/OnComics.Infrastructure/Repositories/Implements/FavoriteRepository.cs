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
        public async Task<(IEnumerable<Favorite>?, IDictionary<Guid, string>, IDictionary<Guid, string>)> GetFavoritesAsync(
            Expression<Func<Favorite, bool>>? filter = null,
            Func<IQueryable<Favorite>, IOrderedQueryable<Favorite>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            var query = _context.Favorites
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
                .Select(i => new
                {
                    Interactions = i,
                    AccountId = i.Account.Id,
                    Fullname = i.Account.Fullname,
                    ComicId = i.Comic.Id,
                    ComicName = i.Comic.Name
                })
                .ToListAsync();

            var interactions = projected.Select(i => i.Interactions).ToList();

            var accounts = projected.ToDictionary(a => a.AccountId, a => a.Fullname);

            var comics = projected.ToDictionary(c => c.ComicId, c => c.ComicName);

            return (interactions, accounts, comics);
        }


        //Get Favorite By Id
        public async Task<(Favorite?, string, string)> GetFavoriteByIdAsync(Guid id)
        {
            var projected = await _context.Favorites
                .AsNoTracking()
                .Where(f => f.Id == id)
                .Select(f => new
                {
                    Favorite = f,
                    Fullname = f.Account.Fullname,
                    ComicName = f.Comic.Name
                })
                .FirstOrDefaultAsync();

            var favorite = projected!.Favorite;

            var fullname = projected!.Fullname;

            var comicName = projected!.ComicName;

            return (favorite, fullname, comicName);
        }

        //Check If Favorite Is Existed
        public async Task<bool> CheckFavoriteExistedAsync(Guid accId, Guid comicId)
        {
            return await _context.Favorites
                .AsNoTracking()
                .AnyAsync(f =>
                    f.AccountId == accId &&
                    f.ComicId == comicId);
        }

        //Count Favorite Record
        public async Task<int> CountFavoriteAsync(Guid id, bool isComic)
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
    }
}
