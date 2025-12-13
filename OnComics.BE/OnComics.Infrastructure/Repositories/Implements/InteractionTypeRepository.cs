using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class InteractionTypeRepository : GenericRepository<Interactiontype>, IInteractionTypeRepository
    {
        public InteractionTypeRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get All Interaction Types
        public async Task<IEnumerable<Interactiontype>?> GetInteractiontypesAsync()
        {
            try
            {
                return await _context.Interactiontypes
                     .AsNoTracking()
                     .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Check If Type Name Is Existed
        public async Task<bool> CheckTypeNameExistedAsync(string name)
        {
            try
            {
                return await _context.Interactiontypes
                    .AsNoTracking()
                    .AnyAsync(t => t.Name.Equals(name));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
