using OnComics.Infrastructure.Domains;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class ChapterSourceRepository : GenericRepository<Chaptersource>, IChapterSourceRepository
    {
        public ChapterSourceRepository(OnComicsDatabaseContext context) : base(context)
        {
        }
    }
}
