using OnComics.Application.Enums.LeaderboardType;
using OnComics.Application.Models.Request.Common;
using System.ComponentModel;

namespace OnComics.Application.Models.Request.LeaderboardType
{
    public class GetLdbTypeReq : GetReq
    {
        [DefaultValue(null)]
        public LdbTypeSortOption? SortBy { get; set; }

        [DefaultValue(null)]
        public LdbTypeStatus? Status { get; set; }
    }
}
