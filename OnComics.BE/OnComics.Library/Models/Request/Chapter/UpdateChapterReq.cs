using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Chapter
{
    public class UpdateChapterReq
    {
        [Required]
        public int ComicId { get; set; }

        [Required]
        public int ChapNo { get; set; }

        public string? Name { get; set; } = string.Empty;
    }
}
