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
            try
            {
                return await _context.Chaptersources
                    .AsNoTracking()
                    .Where(s => s.ChapterId == chapterId)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
