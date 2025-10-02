using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class InteractionRepository : GenericRepository<Interaction>, IInteractionRepository
    {
        public InteractionRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get All Interactions
        public async Task<(IEnumerable<Interaction>?, Dictionary<int, string>, Dictionary<int, string>)> GetInteractionsAsync(
            Expression<Func<Interaction, bool>>? filter = null,
            Func<IQueryable<Interaction>, IOrderedQueryable<Interaction>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            var query = _context.Interactions
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (pageNumber.HasValue && pageSize.HasValue)
                query = query.Skip((pageNumber.Value - 1) * pageSize.Value)
                             .Take(pageSize.Value);

            var projected = await query
                .Select(i => new
                {
                    Interactions = i,
                    AccountId = i.Account.Id,
                    Fullname = i.Account.Fullname,
                    CommentId = i.Comment.Id,
                    CommentAuthor = i.Comment.Account.Fullname
                })
                .ToListAsync();

            var interactions = projected.Select(i => i.Interactions).ToList();

            var accounts = projected.ToDictionary(a => a.AccountId, a => a.Fullname);

            var comments = projected.ToDictionary(c => c.CommentId, c => c.CommentAuthor);

            return (interactions, accounts, comments);
        }

        //Get Interaction By Id
        public async Task<(Interaction?, string, string)> GetInteractionById(int id)
        {
            var interaction = await _context.Interactions
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            string fullname = interaction!.Account.Fullname;

            string author = interaction!.Comment.Account.Fullname;

            return (interaction, fullname, author);
        }

        //Check If Interaction Is Existed
        public async Task<bool> CheckInteractionExistedAsync(int accId, int cmtId)
        {
            return await _context.Interactions
                .AsNoTracking()
                .AnyAsync(i => i.AccountId == accId &&
                    i.CommentId == cmtId);
        }

        public async Task<int> CountInteractionAsync(int? id = null)
        {
            if (id.HasValue)
            {
                return await _context.Interactions
                    .AsNoTracking()
                    .CountAsync(i => i.AccountId == id);
            }
            else
            {
                return await _context.Interactions
                    .AsNoTracking()
                    .CountAsync();
            }
        }
    }
}
