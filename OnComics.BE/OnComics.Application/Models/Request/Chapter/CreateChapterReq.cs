using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Chapter
{
    public class CreateChapterReq
    {
        [Required]
        public Guid ComicId { get; set; }

        public string? Name { get; set; } = string.Empty;
    }
}
