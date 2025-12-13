using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class AttachmentRepository : GenericRepository<Attachment>, IAttachmentRepsitory
    {
        public AttachmentRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Guid>?> GetAttachIdsByCmtIdAsync(Guid id)
        {
            try
            {
                return await _context.Attachments
                    .AsNoTracking()
                    .Where(a => a.CommentId == id)
                    .Select(a => a.Id)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
