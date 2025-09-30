using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Comment
{
    public class CreateCommentReq
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        public int ComicId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
