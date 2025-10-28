using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class ChapterSourceRepository : GenericRepository<Chaptersource>, IChapterSourceRepository
    {
        public ChapterSourceRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get Chapter Source By Chapter Id
        public async Task<IEnumerable<Chaptersource>?> GetSourcesByChapterIdAsync(Guid chapterId)
        {
            return await _context.Chaptersources
                .AsNoTracking()
                .Where(s => s.ChapterId == chapterId)
                .ToListAsync();
        }

        //Get All Chapter Source To Dictionary
        public async Task<Dictionary<Guid, int>> GetChapterSourcesAsync()
        {
            return await _context.Chaptersources
                .AsNoTracking()
                .ToDictionaryAsync(
                    s => s.ChapterId,
                    s => s.Arrangement);
        }

        //Check If Chapter Source is Existed
        public async Task<bool> CheckChapterSourceAsync(Guid chapterId, int arrangement)
        {
            return await _context.Chaptersources
                .AsNoTracking()
                .AnyAsync(s =>
                    s.ChapterId == chapterId &&
                    s.Arrangement == arrangement);
        }

        //Count Chapter Source By Chapter Id
        public async Task<int> CountSourceByChapterId(Guid chapterId)
        {
            return await _context.Chaptersources
                .AsNoTracking()
                .CountAsync(s => s.ChapterId == chapterId);
        }
    }
}
