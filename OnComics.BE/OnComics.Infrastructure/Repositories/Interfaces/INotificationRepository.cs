using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Guid>> GetNotifcationIdsAsync();

        Task<IEnumerable<Notification>?> GetNotificationsByIdsAsync(List<Guid> ids);

        Task<IEnumerable<Guid>?> GetUnReadNotiIdsAsync();

        Task<int> MarkReadNotificationsAsync();
    }
}
