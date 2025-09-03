using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Comic
{
    public class CreateComicReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        public string ThumbnailUrl { get; set; } = string.Empty;

        [Required]
        public bool IsNovel { get; set; }
    }
}