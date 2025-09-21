using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Comic
{
    public class UpdateComicReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string ThumbnailUrl { get; set; } = string.Empty;

        [Required]
        public bool IsNovel { get; set; }
    }
}