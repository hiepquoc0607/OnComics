using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly OnComicsDatabaseContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(OnComicsDatabaseContext context)
        {
            _context = new OnComicsDatabaseContext();
            _dbSet = _context.Set<T>();

        }

        //Get All
        public async Task<IEnumerable<T>?> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            IQueryable<T> query = _dbSet
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (pageNumber.HasValue && pageSize.HasValue)
                query = query.Skip((pageNumber.Value - 1) * pageSize.Value)
                             .Take(pageSize.Value);

            return await query.ToListAsync();
        }

        //Get By Id
        public async Task<T?> GetByIdAsync(object id, bool isTracking)
        {
            switch (isTracking)
            {
                case true:
                    return await _dbSet
                        .FirstOrDefaultAsync(e => EF
                            .Property<object>(e, "Id")
                            .Equals(id));

                case false:
                    return await _dbSet
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => EF
                            .Property<object>(e, "Id")
                            .Equals(id));
            }
        }

        //Insert
        public async Task InsertAsync(T entity, bool isSaving)
        {
            await _dbSet.AddAsync(entity);

            if (isSaving)
                await _context.SaveChangesAsync();
        }

        // Bulk Insert Range
        public async Task BulkInsertAsync(IEnumerable<T> entities)
        {
            // For high performance bulk insert (require EFBulk Extensions package):
            await _context.BulkInsertAsync(entities);
        }

        //Update
        public async Task UpdateAsync(T entity, bool isSaving)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            if (isSaving)
                await _context.SaveChangesAsync();
        }

        //Bulk Update Range
        public async Task BulkUpdateAsync(IEnumerable<T> entities)
        {
            // For high performance bulk insert (require EFBulk Extensions package):
            await _context.BulkUpdateAsync(entities);
        }

        //Delete Ojbect
        public async Task DeleteAsync(T entity, bool isSaving)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);

            _dbSet.Remove(entity);

            if (isSaving)
                await _context.SaveChangesAsync();
        }

        //Bulk Delete Range
        public async Task BulkDeleteAsync(IEnumerable<T> entities)
        {
            // For high performance bulk delete (require EFBulk Extensions package):
            await _context.BulkDeleteAsync(entities);
        }

        //Save Change
        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }

        //Run Transaction Operation
        public async Task RunTransactionAsync(Func<Task> operations)
        {
            using (var transaction = await _context.Database
                .BeginTransactionAsync())
            {
                try
                {
                    await operations();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        //Count Total Record Of An Entity
        public async Task<int> CountRecordAsync(Expression<Func<T, bool>>? filter)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync();
        }
    }
}
