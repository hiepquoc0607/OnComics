using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get All Comments
        public async Task<(IEnumerable<Comment>?, IDictionary<Guid, string>, IDictionary<Guid, string>)> GetCommentsAsync(
            Expression<Func<Comment, bool>>? filter = null,
            Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            var query = _context.Comments
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
                    .Select(c => new
                    {
                        Comments = c,
                        AccountId = c.Account.Id,
                        Fullname = c.Account.Fullname,
                        ComicId = c.Comic.Id,
                        ComicName = c.Comic.Name
                    })
                    .ToListAsync();

            var comments = projected.Select(c => c.Comments).ToList();

            var accounts = projected.ToDictionary(a => a.AccountId, a => a.Fullname);

            var comics = projected.ToDictionary(c => c.ComicId, c => c.ComicName);

            return (comments, accounts, comics);
        }

        //Get Reply Comment By Main Comment Id
        public async Task<(IEnumerable<Comment>?, IDictionary<Guid, string>)> GetReplyCommentsAsync(Guid id)
        {
            var projected = await _context.Comments
                .AsNoTracking()
                .Where(c => c.MainCmtId == id)
                .Select(c => new
                {
                    Comments = c,
                    AccountId = c.Account.Id,
                    Fullname = c.Account.Fullname
                })
                .ToListAsync();

            var comments = projected.Select(c => c.Comments).ToList();

            var accounts = projected.ToDictionary(a => a.AccountId, a => a.Fullname);

            return (comments, accounts);
        }

        //Check If Comment Is Existed
        public async Task<bool> CheckCommentExistedAsync(Guid accId, Guid comicId)
        {
            return await _context.Comments
                .AsNoTracking()
                .AnyAsync(c =>
                    c.AccountId == accId &&
                    c.ComicId == comicId);
        }

        //Count Comment Data By Account Id
        public async Task<int> CountCommentAsync(Guid id, bool isComicId = false)
        {
            switch (isComicId)
            {
                case true:
                    return await _context.Comments
                        .AsNoTracking()
                        .Where(c => c.ComicId == id)
                        .CountAsync();
                default:
                    return await _context.Comments
                        .AsNoTracking()
                        .Where(c => c.AccountId == id)
                        .CountAsync();
            }
        }
    }
}
