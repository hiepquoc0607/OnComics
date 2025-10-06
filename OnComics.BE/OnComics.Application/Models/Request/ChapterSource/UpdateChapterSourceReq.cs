using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.ChapterSource
{
    public class UpdateChapterSourceReq
    {
        [Required]
        public string SrcUrl { get; set; } = null!;

        [Required]
        public int Arrangement { get; set; }
    }
}
