using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Chapter
{
    public enum ChapStatus
    {
        ACTIVE,
        INACTIVE,
        ERROR
    }

    public class GetChapterReq
    {
        [Required]
        public int ComicId { get; set; }

        [DefaultValue(null)]
        public ChapStatus? Status { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsDescending { get; set; }

        [Required]
        [DefaultValue(10)]
        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; }

        [Required]
        [DefaultValue(1)]
        [Range(1, int.MaxValue)]
        public int PageNum { get; set; }
    }
}
