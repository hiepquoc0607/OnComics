using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Comic
{
    public enum ComicSortOptions
    {
        NAME,
        AUTHOR,
        RELEASE,
        UPDATE,
        RATING,
        FAVORITE,
        DATE,
        WEEK,
        MONTH,
        TOTAL,
        NOVEL,
        STATUS
    }

    public enum ComicStatus
    {
        UPCOMING,
        ONGOING,
        FINISHED
    }

    public class GetComicReq
    {
        public string? SearchKey { get; set; }

        [DefaultValue(null)]
        public ComicSortOptions? SortBy { get; set; }

        [DefaultValue(null)]
        public ComicStatus? Status { get; set; }

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