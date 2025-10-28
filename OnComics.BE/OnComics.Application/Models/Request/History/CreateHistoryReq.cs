using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.History
{
    public class CreateHistoryReq
    {
        [Required]
        public Guid ChapterId { get; set; }
    }
}
