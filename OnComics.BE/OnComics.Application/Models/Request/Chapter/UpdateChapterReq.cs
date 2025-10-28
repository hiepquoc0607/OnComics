using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Chapter
{
    public class UpdateChapterReq
    {
        [Required]
        public Guid ComicId { get; set; }

        [Required]
        public int ChapNo { get; set; }

        public string? Name { get; set; } = string.Empty;
    }
}
