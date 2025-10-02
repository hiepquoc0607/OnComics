using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class ComicCategoryRepository : GenericRepository<Comiccategory>, IComicCategoryRepository
    {
        public ComicCategoryRepository(OnComicsDatabaseContext context) : base(context)
        {
        }
    }
}
