using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>?> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<T?> GetByIdAsync(object id, bool isTracking);

        Task InsertAsync(T entity);

        Task BulkInsertRangeAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);

        Task BulkUpdateRangeAsync(IEnumerable<T> entities);

        Task DeleteAsync(T entity);

        Task DeleteAsync(object id);

        Task BulkDeleteRangeAsync(IEnumerable<T> entities);

        Task RunTransactionAsync(Func<Task> operations);

        Task<int> CountRecordAsync(Expression<Func<T, bool>>? filter);
    }
}
