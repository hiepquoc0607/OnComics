using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.ComicRating
{
    public class CreateComicRatingReq
    {
        [Required]
        public Guid ComicId { get; set; }

        [Required]
        [Range(1.0, 5.0)]
        public double Rating { get; set; }
    }
}
