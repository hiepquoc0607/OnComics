using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        Task<FavoritesInfo> GetFavoritesAsync(
            Expression<Func<Favorite, bool>>? filter = null,
            Func<IQueryable<Favorite>, IOrderedQueryable<Favorite>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<bool> CheckFavoriteExistedAsync(Guid accId, Guid comicId);

        Task<int> CountFavoriteAsync(Guid id, bool isComic);
    }

    public record FavoritesInfo(
        IEnumerable<Favorite>? Favorites,
        IDictionary<Guid, (Guid, string)>? Accounts,
        IDictionary<Guid, (Guid, string)>? Comics);
}
