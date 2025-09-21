using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Domains;
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
        public async Task<int> CountChapterByComicIdAsync(int id)
        {
            return await _context.Chapters
                .AsNoTracking()
                .Where(c => c.ComicId == id)
                .CountAsync();
        }

        //Get Max Chapter ChapNo By ComicId
        public async Task<int> GetMaxChapNoByComicIdAsync(int id)
        {
            return await _context.Chapters
                .AsNoTracking()
                .Where(c => c.ComicId == id)
                .MaxAsync(c => c.ChapNo);
        }

        //Get Max Chapter ChapNo For Each ComicIds
        public async Task<Dictionary<int, int>> GetMaxChapNosByComicIdsAsync(int[] ids)
        {
            return await _context.Chapters
                .AsNoTracking()
                .GroupBy(c => c.ComicId)
                .ToDictionaryAsync(
                    c => c.Key,
                    c => c.Max(ct => ct.ChapNo));
        }
    }
}
