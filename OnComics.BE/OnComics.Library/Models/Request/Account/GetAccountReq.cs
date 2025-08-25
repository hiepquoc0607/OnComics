using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Account
{
    public enum AccSortOption
    {
        EMAIL,
        FULLNAME,
        STATUS
    }

    public enum AccStatus
    {
        ACTIVE,
        INACTIVE
    }

    public class GetAccountReq
    {
        public string? SearchKey { get; set; }

        [DefaultValue(null)]
        public AccSortOption? SortBy { get; set; }

        [DefaultValue(null)]
        public AccStatus? Status { get; set; }

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
