using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class ChapterRepository : GenericRepository<Chapter>, IChapterRepository
    {
        public ChapterRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Count Chapter Record By Comic Id
        public async Task<int> CountChapterByComicIdAsync(Guid id)
        {
            return await _context.Chapters
                .AsNoTracking()
                .Where(c => c.ComicId == id)
                .CountAsync();
        }

        //Get Max Chapter ChapNo By ComicId
        public async Task<int> GetMaxChapNoByComicIdAsync(Guid id)
        {
            return await _context.Chapters
                .AsNoTracking()
                .Where(c => c.ComicId == id)
                .MaxAsync(c => c.ChapNo);
        }

        //Get Max Chapter ChapNo For Each ComicIds
        public async Task<IDictionary<Guid, int>> GetMaxChapNosByComicIdsAsync(Guid[] ids)
        {
            return await _context.Chapters
                .AsNoTracking()
                .Where(c => ids.Contains(c.Id))
                .GroupBy(c => c.ComicId)
                .ToDictionaryAsync(
                    c => c.Key,
                    c => c.Where(ct => ct.Id == c.Key)
                        .Max(ct => ct.ChapNo));
        }
    }
}
