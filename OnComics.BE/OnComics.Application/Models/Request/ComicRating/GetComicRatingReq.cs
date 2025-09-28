using OnComics.Application.Enums.ComicRating;
using OnComics.Application.Models.Request.Common;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.ComicRating
{
    public class GetComicRatingReq : GetReq
    {
        public RatingSortOption? SortBy { get; set; }
    }
}
