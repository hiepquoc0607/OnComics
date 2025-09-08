using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Chapter
{
    public class CreateChapterReq
    {
        [Required]
        public int ComicId { get; set; }

        public string? Name { get; set; } = string.Empty;
    }
}
