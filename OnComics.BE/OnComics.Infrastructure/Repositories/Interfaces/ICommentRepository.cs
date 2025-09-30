using OnComics.Infrastructure.Domains;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<(IEnumerable<Comment>?, Dictionary<int, string>, Dictionary<int, string>)> GetCommentsAsync(
            Expression<Func<Comment, bool>>? filter = null,
            Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<(IEnumerable<Comment>?, Dictionary<int, string>)> GetReplyCommentsAsync(int id);

        Task<bool> CheckCommentExistedAsync(int accId, int comicId);

        Task<int> CountCommentByAccountId(int id);

        Task<int> CountCommentByComicId(int id);
    }
}
