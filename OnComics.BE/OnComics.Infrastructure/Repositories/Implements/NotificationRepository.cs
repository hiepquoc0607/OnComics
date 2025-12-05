using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get All Notifcation Ids
        public async Task<IEnumerable<Guid>> GetNotifcationIdsAsync()
        {
            return await _context.Notifications
                 .AsNoTracking()
                 .Select(n => n.Id)
                 .ToListAsync();
        }

        //Get Notifcations By Ids
        public async Task<IEnumerable<Notification>?> GetNotificationsByIdsAsync(List<Guid> ids)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(n => ids.Contains(n.Id))
                .ToListAsync();
        }

        //Mask Read 20 Notification
        public async Task<int> MarkReadNotificationsAsync()
        {
            return await _context.Notifications
                .Where(n => n.IsRead == false)
                .Take(20)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }
    }
}
