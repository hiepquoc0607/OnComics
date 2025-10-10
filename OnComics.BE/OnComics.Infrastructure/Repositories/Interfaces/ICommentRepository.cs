using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<(IEnumerable<Comment>?, IDictionary<int, string>, IDictionary<int, string>)> GetCommentsAsync(
            Expression<Func<Comment, bool>>? filter = null,
            Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<(IEnumerable<Comment>?, IDictionary<int, string>)> GetReplyCommentsAsync(int id);

        Task<bool> CheckCommentExistedAsync(int accId, int comicId);

        Task<int> CountCommentAsync(int id, bool isComicId = false);
    }
}
