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
        public async Task<InteractionsInfo> GetInteractionsAsync(
            Expression<Func<Interaction, bool>>? filter = null,
            Func<IQueryable<Interaction>, IOrderedQueryable<Interaction>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            try
            {
                var query = _context.Interactions
                    .AsNoTracking()
                    .AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                if (query.ToListAsync() == null)
                    return new InteractionsInfo(null, null, null, null);

                if (orderBy != null)
                    query = orderBy(query);

                if (pageNumber.HasValue && pageSize.HasValue)
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value)
                                 .Take(pageSize.Value);

                var projected = await query
                    .Select(i => new
                    {
                        Interactions = i,
                        InteractionId = i.Id,
                        AccountId = i.Account.Id,
                        Fullname = i.Account.Fullname,
                        CommentId = i.Comment.Id,
                        CommentAuthor = i.Comment.Account.Fullname,
                        Type = i.Type
                    })
                    .ToListAsync();

                var interactions = projected.Select(i => i.Interactions).ToList();

                var accounts = projected
                    .GroupBy(a => a.InteractionId)
                    .ToDictionary(
                        a => a.Key, a => (a.First().AccountId, a.First().Fullname));

                var comments = projected
                    .GroupBy(c => c.InteractionId)
                    .ToDictionary(
                        c => c.Key, c => (c.First().CommentId, c.First().CommentAuthor));

                var types = projected
                    .GroupBy(t => t.InteractionId)
                    .ToDictionary(
                        t => t.Key, t => t.First().Type);

                return new InteractionsInfo(interactions, accounts, comments, types);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get Interaction By Id
        public async Task<InteractionInfo> GetInteractionById(Guid id)
        {
            try
            {
                var projected = await _context.Interactions
                    .AsNoTracking()
                    .Where(i => i.Id == id)
                    .Select(i => new
                    {
                        Interaction = i,
                        Fullname = i.Account.Fullname,
                        Author = i.Comment.Account.Fullname,
                        Type = i.Type
                    })
                    .FirstOrDefaultAsync();

                if (projected == null)
                    return new InteractionInfo(null, string.Empty, string.Empty, null);

                var interaction = projected.Interaction;

                string fullname = projected.Fullname;

                string author = projected.Author;

                var type = projected.Type;

                return new InteractionInfo(interaction, fullname, author, type);
            }
            catch (Exception) { throw; }
        }

        //Check If Interaction Is Existed
        public async Task<bool> CheckInteractionExistedAsync(Guid accId, Guid cmtId)
        {
            try
            {
                return await _context.Interactions
                    .AsNoTracking()
                    .AnyAsync(i => i.AccountId == accId &&
                        i.CommentId == cmtId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Count Interaction Record
        public async Task<int> CountInteractionAsync(Guid? id = null)
        {
            try
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}
