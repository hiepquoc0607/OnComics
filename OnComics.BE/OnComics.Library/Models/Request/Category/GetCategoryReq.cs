using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Category
{
    public enum CateSortOption
    {
        NAME,
        STATUS
    }

    public enum CateStatus
    {
        ACTIVE,
        INACTIVE
    }

    public class GetCategoryReq
    {
        public string? SearchKey { get; set; }

        [DefaultValue(null)]
        public CateSortOption? SortBy { get; set; }

        [DefaultValue(null)]
        public CateStatus? Status { get; set; }

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
