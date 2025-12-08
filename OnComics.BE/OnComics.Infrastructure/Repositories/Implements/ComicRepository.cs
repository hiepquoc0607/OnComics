using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class ComicRepository : GenericRepository<Comic>, IComicRepository
    {
        public ComicRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get All Comics
        public async Task<IEnumerable<Comic>?> GetComicsAsync(
            Guid? categoryId,
            Expression<Func<Comic, bool>>? filter = null,
            Func<IQueryable<Comic>, IOrderedQueryable<Comic>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            var query = _context.Comics
                .AsNoTracking()
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query
                    .Include(c => c.Comiccategories)
                        .ThenInclude(cc => cc.Category)
                    .Where(c => c.Comiccategories
                        .Any(cc => cc.CategoryId == categoryId));
            }

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (pageNumber.HasValue && pageSize.HasValue)
                query = query.Skip((pageNumber.Value - 1) * pageSize.Value)
                             .Take(pageSize.Value);

            return await query.ToListAsync();
        }

        //Get Comic By Id
        public async Task<(Comic?, IEnumerable<Category?>)> GetComicByIdAsync(Guid id)
        {
            var projected = await _context.Comics
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    Comic = c,
                    Categories = c.Comiccategories
                        .Select(cc => new
                        {
                            cc.Category.Id,
                            cc.Category.Name
                        })
                })
                .FirstOrDefaultAsync();

            if (projected == null)
                return (null, new List<Category>());

            var comic = projected.Comic;
            var categories = projected.Categories
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToList();

            return (comic, categories);
        }


        //Check If Comic Is Existed
        public async Task<bool> CheckComicExistedAsync(string name, string author)
        {
            return await _context.Comics
                .AsNoTracking()
                .AnyAsync(c =>
                    EF.Functions.Like(c.Name, $"%{name}%") &&
                    EF.Functions.Like(c.Author, $"%{author}%"));
        }

        //Check If Comic Is Existed By Id
        public async Task<bool> CheckComicIdAsync(Guid id)
        {
            return await _context.Comics
                .AsNoTracking()
                .AnyAsync(c => c.Id == id);
        }

        //Get All Comic Ids
        public async Task<Guid[]> GetComicIdsAsync()
        {
            return await _context.Comics
                .AsNoTracking()
                .Select(c => c.Id)
                .ToArrayAsync();
        }

        public async Task<int> CountComicsByCateId(
            Guid categoryId,
            Expression<Func<Comic, bool>>? filter = null)
        {
            var query = _context.Comics
                .AsNoTracking()
                .Include(c => c.Comiccategories)
                    .ThenInclude(cc => cc.Category)
                .Where(c => c.Comiccategories
                    .Any(cc => cc.CategoryId == categoryId))
                .AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync();
        }

        //Reset Day Read Number Of All Comic
        public async Task ResetDayReadNumAsync()
        {
            await _context.Comics
               .Where(c => c.DayReadNum != 0)
               .ExecuteUpdateAsync(s => s.SetProperty(n => n.DayReadNum, 0));
        }

        //Reset Week Read Number Of All Comic
        public async Task ResetWeekReadNumAsync()
        {
            await _context.Comics
                .Where(c => c.WeekReadNum != 0)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.WeekReadNum, 0));
        }

        //Reset Month Read Number Of All Comic
        public async Task ResetMonthReadNumAsync()
        {
            await _context.Comics
               .Where(c => c.MonthReadNum != 0)
               .ExecuteUpdateAsync(s => s.SetProperty(n => n.MonthReadNum, 0));
        }
    }
}
