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
            try
            {
                return await _context.Notifications
                     .AsNoTracking()
                     .Select(n => n.Id)
                     .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get Notifcations By Ids
        public async Task<IEnumerable<Notification>?> GetNotificationsByIdsAsync(List<Guid> ids)
        {
            try
            {
                return await _context.Notifications
                    .AsNoTracking()
                    .Where(n => ids.Contains(n.Id))
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get 20 First Unread Notification
        public async Task<IEnumerable<Guid>?> GetUnReadNotiIdsAsync()
        {
            try
            {
                return await _context.Notifications
                    .AsNoTracking()
                    .Where(n => n.IsRead == false)
                    .Select(n => n.Id)
                    .Take(20)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Mask Read 20 Notification
        public async Task<int> MarkReadNotificationsAsync()
        {
            try
            {
                return await _context.Notifications
                    .Where(n => n.IsRead == false)
                    .Take(20)
                    .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
