using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IHistoryRepository : IGenericRepository<History>
    {
        Task<HistoriesInfo> GetHistoriesAsync(
            Expression<Func<History, bool>>? filter = null,
            Func<IQueryable<History>, IOrderedQueryable<History>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<IEnumerable<History>?> GetHistoriesByAccountIdAsync(Guid id);

        Task<bool> CheckHistoryExistedAsync(Guid accId, Guid chapterId);

        Task<int> CountHistoryAsync(Guid id, bool isComicId);
    }

    public record HistoriesInfo(
        IEnumerable<History>? Histories,
        IDictionary<Guid, (Guid, string)>? Accounts,
        IDictionary<Guid, (Guid, string)>? Comics);
}
