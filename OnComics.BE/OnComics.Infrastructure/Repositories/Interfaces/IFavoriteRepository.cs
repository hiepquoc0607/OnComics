using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        Task<(IEnumerable<Favorite>?, Dictionary<int, string>, Dictionary<int, string>)> GetFavoritesAsync(
            Expression<Func<Favorite, bool>>? filter = null,
            Func<IQueryable<Favorite>, IOrderedQueryable<Favorite>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<(Favorite?, string, string)> GetFavoriteByIdAsync(int id);

        Task<bool> CheckFavoriteExistedAsync(int accId, int comicId);

        Task<int> CountFavoriteAsync(int id, bool isComic);
    }
}
