using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IComicRepository : IGenericRepository<Comic>
    {
        Task<bool> CheckComicExistedAsync(string name, string author);

        Task<bool> CheckComicIdAsync(Guid id);

        Task<Guid[]> GetComicIdsAsync();
    }
}
