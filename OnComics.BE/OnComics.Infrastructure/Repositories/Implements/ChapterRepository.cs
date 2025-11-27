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

        //Get Chapter By Id
        public async Task<SourceInfo> GetChapterByIdAsync(Guid id)
        {
            var projected = await _context.Chapters
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    Chapter = c,
                    Source = c.Chaptersources
                        .Select(cs => new
                        {
                            cs.Id,
                            cs.SrcUrl,
                            cs.IsImage,
                            cs.Arrangement
                        })
                })
                .FirstOrDefaultAsync();

            var chapter = projected!.Chapter;
            var sources = projected.Source
                .Select(s => new Chaptersource
                {
                    Id = s.Id,
                    SrcUrl = s.SrcUrl,
                    IsImage = s.IsImage,
                    Arrangement = s.Arrangement
                })
                .OrderBy(s => s.Arrangement)
                .ToList();

            return new SourceInfo(chapter, sources);
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
    }
}
