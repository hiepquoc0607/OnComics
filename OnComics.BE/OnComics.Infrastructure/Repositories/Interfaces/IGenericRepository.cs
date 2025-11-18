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

        Task InsertAsync(T entity, bool isSaving);

        Task BulkInsertAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity, bool isSaving);

        Task BulkUpdateAsync(IEnumerable<T> entities);

        Task DeleteAsync(T entity, bool isSaving);

        Task BulkDeleteAsync(IEnumerable<T> entities);

        Task SaveChangeAsync();

        Task RunTransactionAsync(Func<Task> operations);

        Task<int> CountRecordAsync(Expression<Func<T, bool>>? filter);
    }
}
