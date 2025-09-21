using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Domains;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class LeaderboardTypeRepository : GenericRepository<Leaderboardtype>, ILeaderboardTypeRepository
    {
        public LeaderboardTypeRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Check If Type Name Is Existed
        public async Task<bool> CheckTypeNameExistedAsync(string name)
        {
            return await _context.Leaderboardtypes
                .AsNoTracking()
                .AnyAsync(t => t.Name.Equals(name));
        }
    }
}
