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
        public async Task<CommentsInfo> GetCommentsAsync(
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
                        CommentId = c.Id,
                        AccountId = c.Account.Id,
                        Fullname = c.Account.Fullname,
                        ComicId = c.Comic.Id,
                        ComicName = c.Comic.Name,
                        Attachments = c.Attachments
                    })
                    .ToListAsync();

            var comments = projected.Select(c => c.Comments).ToList();

            var accounts = projected
                .GroupBy(a => a.CommentId)
                .ToDictionary(
                    a => a.Key,
                    a => (a.First().AccountId, a.First().Fullname)
                );

            var comics = projected
                .GroupBy(c => c.CommentId)
                .ToDictionary(
                    c => c.Key,
                    c => (c.First().ComicId, c.First().ComicName)
                );

            var attachments = projected
                .GroupBy(a => a.CommentId)
                .ToDictionary(
                    a => a.Key,
                    a => a.First().Attachments.ToList()
                );

            return new CommentsInfo(comments, accounts, comics, attachments!); ;
        }

        //Get Reply Comment By Main Comment Id
        public async Task<RepliesInfo> GetReplyCommentsAsync(Guid id)
        {
            var projected = await _context.Comments
                .AsNoTracking()
                .Where(c => c.MainCmtId == id)
                .Select(c => new
                {
                    Comments = c,
                    CommentId = c.Id,
                    AccountId = c.Account.Id,
                    Fullname = c.Account.Fullname,
                    Attachments = c.Attachments
                })
                .ToListAsync();

            var comments = projected.Select(c => c.Comments).ToList();

            var accounts = projected
                .GroupBy(a => a.CommentId)
                .ToDictionary(
                    a => a.Key,
                    a => (a.First().AccountId, a.First().Fullname)
                );

            var attachments = projected
                .GroupBy(a => a.CommentId)
                .ToDictionary(
                    a => a.Key,
                    a => a.First().Attachments.ToList()
                );

            return new RepliesInfo(comments, accounts, attachments!);
        }

        //Count Comment Data By Account Id
        public async Task<int> CountCommentAsync(Guid id, bool isComicId)
        {
            switch (isComicId)
            {
                case true:
                    return await _context.Comments
                        .AsNoTracking()
                        .Where(c => c.ComicId == id)
                        .CountAsync();
                case false:
                    return await _context.Comments
                        .AsNoTracking()
                        .Where(c => c.AccountId == id)
                        .CountAsync();
            }
        }
    }
}
