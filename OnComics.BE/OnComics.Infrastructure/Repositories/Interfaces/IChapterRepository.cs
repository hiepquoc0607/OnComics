using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IChapterRepository : IGenericRepository<Chapter>
    {
        Task<(Chapter, IEnumerable<Chaptersource>?)> GetChapterByIdAsync(Guid id);

        Task<int> CountChapterByComicIdAsync(Guid id);

        Task<int> GetMaxChapNoByComicIdAsync(Guid id);
    }
}
