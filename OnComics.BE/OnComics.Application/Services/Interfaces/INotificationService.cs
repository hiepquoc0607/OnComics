using OnComics.Application.Models.Request.Notification;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.Notification;

namespace OnComics.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task<ObjectResponse<IEnumerable<NotificationRes>?>> GetNotificationsAsync(GetNotificationReq getNotificationReq);

        Task<ObjectResponse<NotificationRes?>> GetNotificationByIdAsync(Guid id);

        Task<VoidResponse> DeleteNotificationAsync(Guid id);

        Task<VoidResponse> DeleteNotificationsAsync(List<Guid> ids);

        Task<VoidResponse> MarkReadNotificationsAsync();
    }
}
