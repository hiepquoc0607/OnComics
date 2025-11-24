using OnComics.Application.Enums.InteractionType;
using OnComics.Application.Models.Request.Common;
using System.ComponentModel;

namespace OnComics.Application.Models.Request.InteractionType
{
    public class GetItrTypeReq : GetReq
    {
        [DefaultValue(null)]
        public ItrTypeSortOption? SortBy { get; set; }
    }
}
