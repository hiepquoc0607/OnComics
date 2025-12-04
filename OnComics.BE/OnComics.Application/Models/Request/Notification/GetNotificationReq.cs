using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Notification
{
    public class GetNotificationReq
    {
        [Required]
        [DefaultValue(false)]
        public bool IsDescending { get; set; }

        [Required]
        [DefaultValue(10)]
        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; }

        [Required]
        [DefaultValue(1)]
        [Range(1, int.MaxValue)]
        public int PageNum { get; set; }
    }
}
