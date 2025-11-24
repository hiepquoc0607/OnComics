using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IChapterSourceRepository : IGenericRepository<Chaptersource>
    {
        Task<IEnumerable<Chaptersource>?> GetSourcesByChapterIdAsync(Guid chapterId);

        Task<Dictionary<Guid, int>> GetChapterSourcesAsync();

        Task<bool> CheckChapterSourceAsync(Guid chapterId, int arrangement);

        Task<int> CountSourceByChapterIdAsync(Guid chapterId);
    }
}
