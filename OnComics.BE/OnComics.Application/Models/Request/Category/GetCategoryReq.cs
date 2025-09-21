using OnComics.Application.Enums.Category;
using OnComics.Application.Models.Request.Common;
using System.ComponentModel;

namespace OnComics.Application.Models.Request.Category
{
    public class GetCategoryReq : GetReq
    {
        [DefaultValue(null)]
        public CategorySortOption? SortBy { get; set; }

        [DefaultValue(null)]
        public CategoryStatus? Status { get; set; }
    }
}
