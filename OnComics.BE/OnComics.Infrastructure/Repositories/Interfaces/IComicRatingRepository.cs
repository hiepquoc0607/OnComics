using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IComicRatingRepository : IGenericRepository<Comicrating>
    {
        Task<(IEnumerable<Comicrating>, IDictionary<int, string>, IDictionary<int, string>)> GetRatingsAsync(
            Expression<Func<Comicrating, bool>>? filter = null,
            Func<IQueryable<Comicrating>, IOrderedQueryable<Comicrating>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<Comicrating?> GetRatingByAccIdAndComicIdAsync(int accId, int comicId);

        Task<int> CountRatingByAccountIdAsync(int id);

        Task<int> CountRatingByComicIdAsync(int id);
    }
}
