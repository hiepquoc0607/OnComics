using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OnComics.Application.Models.Request.Notification;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{
    [Route("api/notifications")]
    [Authorize(Policy = "Admin")]
    [ApiController]
    [EnableRateLimiting("BasePolicy")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetNotificationReq getNotificationReq)
        {
            var result = await _notificationService.GetNotificationsAsync(getNotificationReq);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            var result = await _notificationService.GetNotificationByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("mark-read")]
        public async Task<IActionResult> MarkReadAsync()
        {
            var result = await _notificationService.MarkReadNotificationsAsync();

            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("bulk")]
        public async Task<IActionResult> RangeDeleteAsync([FromBody] List<Guid> ids)
        {
            var result = await _notificationService.DeleteNotificationsAsync(ids);

            return StatusCode(result.StatusCode, result);
        }
    }
}
