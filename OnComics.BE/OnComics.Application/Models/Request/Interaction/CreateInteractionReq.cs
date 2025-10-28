using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Interaction
{
    public class CreateInteractionReq
    {
        [Required]
        public Guid CommentId { get; set; }

        [Required]
        public Guid TypeId { get; set; }
    }
}
