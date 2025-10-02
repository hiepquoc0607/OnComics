using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Interaction
{
    public class UpdateInteractionReq
    {
        [Required]
        public int TypeId { get; set; }
    }
}
