using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IHistoryRepository : IGenericRepository<History>
    {
        Task<(IEnumerable<History>?, IDictionary<int, string>, IDictionary<int, string>)> GetHistoriesAsync(
            Expression<Func<History, bool>>? filter = null,
            Func<IQueryable<History>, IOrderedQueryable<History>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<IEnumerable<History>?> GetHistoriesByAccountIdAsync(int id);

        Task<bool> CheckHistoryExistedAsync(int accId, int chapterId);

        Task<int> CountHistoryAsync(int id, bool isComicId);
    }
}
