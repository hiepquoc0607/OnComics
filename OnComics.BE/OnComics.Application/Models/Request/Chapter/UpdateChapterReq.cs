using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Chapter
{
    public class UpdateChapterReq
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ChapNo { get; set; }

        public string? Name { get; set; } = string.Empty;
    }
}
