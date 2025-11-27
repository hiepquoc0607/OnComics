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

        Task BulkInsertAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);

        Task BulkUpdateAsync(IEnumerable<T> entities);

        Task DeleteAsync(T entity);

        Task BulkDeleteAsync(IEnumerable<T> entities);

        Task<int> CountRecordAsync(Expression<Func<T, bool>>? filter);
    }
}
