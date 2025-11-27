using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IComicRatingRepository : IGenericRepository<Comicrating>
    {
        Task<RatingsInfo> GetRatingsAsync(
            Expression<Func<Comicrating, bool>>? filter = null,
            Func<IQueryable<Comicrating>, IOrderedQueryable<Comicrating>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<Comicrating?> GetRatingByAccIdAndComicIdAsync(Guid accId, Guid comicId);

        Task<int> CountRatingAsync(Guid id, bool isComicId);

        Task<decimal> AverageRatingAsync(Guid comicId, double? newRating, decimal? oldRating);
    }

    public record RatingsInfo(
        IEnumerable<Comicrating>? Ratings,
        IDictionary<Guid, string>? Accounts,
        IDictionary<Guid, string>? Comics);
}
