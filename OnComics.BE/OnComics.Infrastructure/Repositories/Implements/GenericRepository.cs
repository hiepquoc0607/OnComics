using EFCore.BulkExtensions;
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
            IQueryable<T> query = _dbSet.AsNoTracking();

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
        public async Task InsertAsync(T entity)
        {
            await _dbSet.AddAsync(entity);

            await _context.SaveChangesAsync();
        }

        // Bulk Insert Range
        public async Task BulkInsertRangeAsync(IEnumerable<T> entities)
        {
            // Default bulk insert of EF:
            //await _dbSet.AddRangeAsync(entities);
            //await _context.SaveChangesAsync();

            // For high performance bulk insert (require EF BulkExtensions package):
            await _context.BulkInsertAsync(entities);
        }

        //Update
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        //Bulk Update Range
        public async Task BulkUpdateRangeAsync(IEnumerable<T> entities)
        {
            await _context.BulkUpdateAsync(entities);
        }

        //Delete Ojbect
        public async Task DeleteAsync(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);

            _dbSet.Remove(entity);

            await _context.SaveChangesAsync();
        }

        //Delete By Id
        public async Task DeleteAsync(object id)
        {
            var entity = await GetByIdAsync(id, true);

            if (entity != null)
                _dbSet.Remove(entity);

            await _context.SaveChangesAsync();
        }

        //Bulk Delete Range
        public async Task BulkDeleteRangeAsync(IEnumerable<T> entities)
        {
            // Default bulk delete of EF:
            //_dbSet.RemoveRange(entities);
            //await _context.SaveChangesAsync();

            // For high performance bulk delete (require EF BulkExtensions package):
            await _context.BulkDeleteAsync(entities);
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
        public async Task<int> CountRecordAsync()
        {
            return await _dbSet.AsNoTracking().CountAsync();
        }
    }
}
