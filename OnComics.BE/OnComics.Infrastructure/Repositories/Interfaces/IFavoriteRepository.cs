using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        Task<(IEnumerable<Favorite>?, IDictionary<Guid, string>, IDictionary<Guid, string>)> GetFavoritesAsync(
            Expression<Func<Favorite, bool>>? filter = null,
            Func<IQueryable<Favorite>, IOrderedQueryable<Favorite>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<(Favorite?, string, string)> GetFavoriteByIdAsync(Guid id);

        Task<bool> CheckFavoriteExistedAsync(Guid accId, Guid comicId);

        Task<int> CountFavoriteAsync(Guid id, bool isComic);
    }
}
