using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IInteractionRepository : IGenericRepository<Interaction>
    {
        Task<(IEnumerable<Interaction>?, IDictionary<int, string>, IDictionary<int, string>)> GetInteractionsAsync(
            Expression<Func<Interaction, bool>>? filter = null,
            Func<IQueryable<Interaction>, IOrderedQueryable<Interaction>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<(Interaction?, string, string)> GetInteractionById(int id);

        Task<bool> CheckInteractionExistedAsync(int accId, int cmtId);

        Task<int> CountInteractionAsync(int? id = null);
    }
}
