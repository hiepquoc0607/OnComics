using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Favorite
{
    public class CreateFavoriteReq
    {
        [Required]
        public Guid ComicId { get; set; }
    }
}
