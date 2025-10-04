using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.History
{
    public class UpdateHistoryReq
    {
        [Required]
        public int ChapterId { get; set; }
    }
}
