using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Interaction
{
    public class CreateInteractionReq
    {
        [Required]
        public int CommentId { get; set; }

        [Required]
        public int TypeId { get; set; }
    }
}
