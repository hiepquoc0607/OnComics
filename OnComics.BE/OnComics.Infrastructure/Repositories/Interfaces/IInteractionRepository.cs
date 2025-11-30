using OnComics.Infrastructure.Entities;
using System.Linq.Expressions;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IInteractionRepository : IGenericRepository<Interaction>
    {
        Task<InteractionsInfo> GetInteractionsAsync(
            Expression<Func<Interaction, bool>>? filter = null,
            Func<IQueryable<Interaction>, IOrderedQueryable<Interaction>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null);

        Task<InteractionInfo> GetInteractionById(Guid id);

        Task<bool> CheckInteractionExistedAsync(Guid accId, Guid cmtId);

        Task<int> CountInteractionAsync(Guid? id = null);
    }

    public record InteractionsInfo(
        IEnumerable<Interaction>? Interactions,
        IDictionary<Guid, (Guid, string)>? Accounts,
        IDictionary<Guid, (Guid, string)>? Comments,
        IDictionary<Guid, Interactiontype>? Types);

    public record InteractionInfo(
        Interaction? Interaction,
        string Fullname,
        string Author,
        Interactiontype? Type);
}
