using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IChapterRepository : IGenericRepository<Chapter>
    {
        Task<int> CountChapterByComicIdAsync(Guid id);

        Task<int> GetMaxChapNoByComicIdAsync(Guid id);

        Task<IDictionary<Guid, int>> GetMaxChapNosByComicIdsAsync(Guid[] ids);
    }
}
