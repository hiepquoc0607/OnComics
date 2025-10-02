using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IInteractionTypeRepository : IGenericRepository<Interactiontype>
    {
        Task<bool> CheckTypeNameExistedAsync(string name);
    }
}
