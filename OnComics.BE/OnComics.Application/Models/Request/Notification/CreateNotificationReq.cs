using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Notification
{
    public class CreateNotificationReq
    {
        [Required]
        public Guid ChapterId { get; set; }
    }
}
