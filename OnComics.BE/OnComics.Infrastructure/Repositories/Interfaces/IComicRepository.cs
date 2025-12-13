using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IComicRepository : IGenericRepository<Comic>
    {
        Task<IEnumerable<Comic>?> GetComicsAsync(
            Guid? categoryId,
            Expression<Func<Comic, bool>>? filter = null,
            Func<IQueryable<Comic>, IOrderedQueryable<Comic>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<ComicInfo> GetComicByIdAsync(Guid id);

        Task<bool> CheckComicExistedAsync(string name, string author);

        Task<bool> CheckComicIdAsync(Guid id);

        Task<int> CountComicsByCateId(
            Guid categoryId,
            Expression<Func<Comic, bool>>? filter = null);

        Task ResetDayReadNumAsync();

        Task ResetWeekReadNumAsync();

        Task ResetMonthReadNumAsync();
    }

    public record ComicInfo(
        Comic? Comic,
        IEnumerable<Category>? Categories);
}
