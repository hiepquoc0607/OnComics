using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Comment
{
    public class UpdateCommentReq
    {

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
