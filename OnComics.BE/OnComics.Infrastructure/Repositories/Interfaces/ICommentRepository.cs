using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<CommentsInfo> GetCommentsAsync(
            Expression<Func<Comment, bool>>? filter = null,
            Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<RepliesInfo> GetReplyCommentsAsync(Guid id);

        Task<int> CountCommentAsync(Guid id, bool isComicId);
    }

    public record CommentsInfo(
        IEnumerable<Comment>? Comments,
        IDictionary<Guid, (Guid, string)>? Accounts,
        IDictionary<Guid, (Guid, string)>? Comics,
        IDictionary<Guid, List<Attachment>?>? Attachments);

    public record RepliesInfo(
        IEnumerable<Comment>? Comments,
        IDictionary<Guid, (Guid, string)>? Accounts,
        IDictionary<Guid, List<Attachment>?>? Attachments);
}
