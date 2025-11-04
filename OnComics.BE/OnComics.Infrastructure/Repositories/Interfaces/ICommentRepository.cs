using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<(IEnumerable<Comment>?, IDictionary<Guid, string>, IDictionary<Guid, string>)> GetCommentsAsync(
            Expression<Func<Comment, bool>>? filter = null,
            Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<(IEnumerable<Comment>?, IDictionary<Guid, string>)> GetReplyCommentsAsync(Guid id);

        Task<bool> CheckCommentExistedAsync(Guid accId, Guid comicId);

        Task<int> CountCommentAsync(Guid id, bool isComicId);
    }
}
