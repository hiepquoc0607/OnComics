using Mapster;
using MapsterMapper;
using Microsoft.IdentityModel.Tokens;
using OnComics.Application.Models.Request.Notification;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.Notification;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public NotificationService(
            INotificationRepository notificationRepository,
            IMapper mapper,
            Util util)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _util = util;
        }

        public async Task<ObjectResponse<IEnumerable<NotificationRes>?>> GetNotificationsAsync(GetNotificationReq getNotificationReq)
        {

            bool isDescending = getNotificationReq.IsDescending;

            int pageNum = getNotificationReq.PageNum;
            int pageIndex = getNotificationReq.PageIndex;

            Func<IQueryable<Notification>, IOrderedQueryable<Notification>>? order = n =>
                getNotificationReq.IsDescending switch
                {
                    true => n.OrderBy(n => n.IsRead == false)
                            .ThenByDescending(n => n.SendTime),
                    _ => n.OrderBy(n => n.IsRead == false)
                            .ThenBy(n => n.SendTime)
                };

            var notis = await _notificationRepository.GetAsync(null, order, pageNum, pageIndex);

            if (notis == null || notis.IsNullOrEmpty())
                return new ObjectResponse<IEnumerable<NotificationRes>?>(
                    (int)HttpStatusCode.NotFound,
                    "Notification Data Empty!");

            var data = notis.Adapt<IEnumerable<NotificationRes>>();

            var totalData = await _notificationRepository.CountRecordAsync(null);
            var toatlPage = (int)Math.Ceiling((decimal)totalData / getNotificationReq.PageIndex);
            var pagination = new Pagination(totalData, pageIndex, pageNum, toatlPage);

            return new ObjectResponse<IEnumerable<NotificationRes>?>(
                (int)HttpStatusCode.OK,
                "Fetch Data Successfully!",
                data,
                pagination);
        }

        public async Task<ObjectResponse<NotificationRes?>> GetNotificationByIdAsync(Guid id)
        {
            try
            {
                var noti = await _notificationRepository.GetByIdAsync(id, true);

                if (noti == null)
                    return new ObjectResponse<NotificationRes?>(
                        (int)HttpStatusCode.NotFound,
                        "Notification Not Found");

                noti.IsRead = true;

                await _notificationRepository.UpdateAsync(noti);

                var data = _mapper.Map<NotificationRes>(noti);

                return new ObjectResponse<NotificationRes?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<NotificationRes?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        public async Task<VoidResponse> DeleteNotificationAsync(Guid id)
        {
            try
            {
                var noti = await _notificationRepository.GetByIdAsync(id, true);

                if (noti == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Notifcation Not Found!");

                await _notificationRepository.DeleteAsync(noti);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Notification Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        public async Task<VoidResponse> DeleteNotificationsAsync(List<Guid> ids)
        {
            try
            {
                var dataIds = await _notificationRepository.GetNotifcationIdsAsync();
                var invalidIds = _util.CompareGuidArray(ids.ToArray(), dataIds.ToArray());

                if (invalidIds != null && invalidIds.Length > 0)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Notification Ids Not Found: " + string.Join(", ", invalidIds));

                var notis = await _notificationRepository.GetNotificationsByIdsAsync(ids);

                if (notis == null || notis.IsNullOrEmpty())
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Notifications Not Found!");

                await _notificationRepository.BulkDeleteAsync(notis);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Notifications Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        public async Task<VoidResponse> MarkReadNotificationsAsync()
        {
            try
            {
                var affectRows = await _notificationRepository.MarkReadNotificationsAsync();

                if (affectRows == 0)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "No Unread Notifcations Found!");

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Mask Read Notification Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }
    }
}
