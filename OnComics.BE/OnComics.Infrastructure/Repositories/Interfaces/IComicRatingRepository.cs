using OnComics.Infrastructure.Domains;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IComicRatingRepository : IGenericRepository<Comicrating>
    {
        Task<Comicrating?> GetRatingByAccIdAndComicIdAsync(int accId, int comicId);

        Task<int> CountRatingByAccountIdAsync(int id);

        Task<int> CountRatingByComicIdAsync(int id);
    }
}
