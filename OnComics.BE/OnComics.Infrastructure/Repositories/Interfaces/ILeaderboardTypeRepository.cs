using OnComics.Infrastructure.Domains;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface ILeaderboardTypeRepository : IGenericRepository<Leaderboardtype>
    {
        Task<bool> CheckTypeNameExistedAsync(string name);
    }
}
