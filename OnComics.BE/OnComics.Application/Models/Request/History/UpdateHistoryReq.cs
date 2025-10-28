using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.History
{
    public class UpdateHistoryReq
    {
        [Required]
        public Guid ChapterId { get; set; }
    }
}
