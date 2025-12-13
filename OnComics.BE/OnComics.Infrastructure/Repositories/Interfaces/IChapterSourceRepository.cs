using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IChapterSourceRepository : IGenericRepository<Chaptersource>
    {
        Task<IEnumerable<Chaptersource>?> GetSourcesByChapterIdAsync(Guid chapterId);
    }
}
