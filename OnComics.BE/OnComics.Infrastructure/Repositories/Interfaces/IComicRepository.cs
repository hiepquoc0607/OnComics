using OnComics.Infrastructure.Domains;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IComicRepository : IGenericRepository<Comic>
    {
        Task<bool> CheckComicExistedAsync(string name, string author);

        Task<bool> CheckComicIdAsync(int id);

        Task<int[]> GetComicIdsAsync();
    }
}
