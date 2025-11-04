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
    }
}
