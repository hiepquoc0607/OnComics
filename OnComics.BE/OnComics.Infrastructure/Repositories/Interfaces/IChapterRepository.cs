using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IChapterRepository : IGenericRepository<Chapter>
    {
        Task<SourceInfo> GetChapterByIdAsync(Guid id);

        Task<int> CountChapterByComicIdAsync(Guid id);

        Task<int> GetMaxChapNoByComicIdAsync(Guid id);
    }

    public record SourceInfo(
        Chapter Chapter,
        IEnumerable<Chaptersource>? Source);
}
