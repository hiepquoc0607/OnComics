using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class HistoryRepository : GenericRepository<History>, IHistoryRepository
    {
        public HistoryRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get All Histories
        public async Task<HistoriesInfo> GetHistoriesAsync(
            Expression<Func<History, bool>>? filter = null,
            Func<IQueryable<History>, IOrderedQueryable<History>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            try
            {
                var query = _context.Histories
                    .AsNoTracking()
                    .AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                if (await query.ToListAsync() == null)
                    return new HistoriesInfo(null, null, null);

                if (orderBy != null)
                    query = orderBy(query);

                if (pageNumber.HasValue && pageSize.HasValue)
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value)
                                 .Take(pageSize.Value);

                var projected = await query
                    .Select(h => new
                    {
                        Histories = h,
                        HistoryId = h.Id,
                        AccountId = h.Account.Id,
                        Fullname = h.Account.Fullname,
                        ComicId = h.Chapter.Comic.Id,
                        ComicName = h.Chapter.Comic.Name
                    })
                    .ToListAsync();

                var histories = projected.Select(i => i.Histories).ToList();

                var accounts = projected
                        .GroupBy(a => a.HistoryId)
                        .ToDictionary(
                            a => a.Key,
                            a => (a.First().AccountId, a.First().Fullname));

                var comics = projected
                    .GroupBy(a => a.HistoryId)
                    .ToDictionary(
                        a => a.Key,
                        a => (a.First().ComicId, a.First().ComicName));

                return new HistoriesInfo(histories, accounts, comics);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get All Histories By Account Id
        public async Task<IEnumerable<History>?> GetHistoriesByAccountIdAsync(Guid id)
        {
            try
            {
                return await _context.Histories
                    .Where(r => r.AccountId == id)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Check If
        public async Task<bool> CheckHistoryExistedAsync(Guid accId, Guid chapterId)
        {
            try
            {
                return await _context.Histories
                    .AsNoTracking()
                    .Where(h =>
                        h.AccountId == accId &&
                        h.ChapterId == chapterId)
                    .AnyAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Count History Record
        public async Task<int> CountHistoryAsync(Guid id, bool isComicId)
        {
            try
            {
                switch (isComicId)
                {
                    case true:
                        return await _context.Histories
                            .AsNoTracking()
                            .CountAsync(h => h.Chapter.Comic.Id == id);
                    default:
                        return await _context.Histories
                           .AsNoTracking()
                           .CountAsync(h => h.AccountId == id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
