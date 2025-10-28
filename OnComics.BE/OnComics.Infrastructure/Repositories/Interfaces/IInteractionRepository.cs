using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IInteractionRepository : IGenericRepository<Interaction>
    {
        Task<(IEnumerable<Interaction>?, IDictionary<Guid, string>, IDictionary<Guid, string>)> GetInteractionsAsync(
            Expression<Func<Interaction, bool>>? filter = null,
            Func<IQueryable<Interaction>, IOrderedQueryable<Interaction>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<(Interaction?, string, string)> GetInteractionById(Guid id);

        Task<bool> CheckInteractionExistedAsync(Guid accId, Guid cmtId);

        Task<int> CountInteractionAsync(Guid? id = null);
    }
}
