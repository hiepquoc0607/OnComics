using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IAttachmentRepsitory : IGenericRepository<Attachment>
    {
        Task<IEnumerable<Guid>?> GetAttachIdsByCmtIdAsync(Guid id);
    }
}
