using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IChapterRepository : IGenericRepository<Chapter>
    {
        Task<int> CountChapterByComicIdAsync(int id);

        Task<int> GetMaxChapNoByComicIdAsync(int id);

        Task<Dictionary<int, int>> GetMaxChapNosByComicIdsAsync(int[] ids);
    }
}
