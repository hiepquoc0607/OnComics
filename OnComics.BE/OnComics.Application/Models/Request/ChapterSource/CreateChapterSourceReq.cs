using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.ChapterSource
{
    public class CreateChapterSourceReq
    {
        [Required]
        public Guid ChapterId { get; set; }

        [Required]
        public string SrcUrl { get; set; } = null!;

        [Required]
        public int Arrangement { get; set; }
    }
}
