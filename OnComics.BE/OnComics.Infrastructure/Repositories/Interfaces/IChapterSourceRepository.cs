using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IChapterSourceRepository : IGenericRepository<Chaptersource>
    {
        Task<IEnumerable<Chaptersource>?> GetSourcesByChapterIdAsync(int chapterId);

        Task<Dictionary<int, int>> GetChapterSourcesAsync();

        Task<bool> CheckChapterSourceAsync(int chapterId, int arrangement);

        Task<int> CountSourceByChapterId(int chapterId);
    }
}
