using System.Linq.Expressions;

namespace OnComics.Repository.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>?> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null,
            bool isTracking = false);

        Task<T?> GetByIdAsync(object id);

        Task InsertAsync(T entity);

        Task InsertRangeAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task DeleteAsync(object id);

        Task DeleteRangeAsync(IEnumerable<T> entities);

        Task RunTransactionAsync(Func<Task> operations);
    }
}
