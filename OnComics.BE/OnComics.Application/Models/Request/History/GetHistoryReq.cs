using OnComics.Application.Enums.History;
using OnComics.Application.Models.Request.Common;
using System.ComponentModel;

namespace OnComics.Application.Models.Request.History
{
    public class GetHistoryReq : GetReq
    {
        [DefaultValue(null)]
        public int? Id { get; set; }

        [DefaultValue(null)]
        public HistoryIdType? IdType { get; set; }

        [DefaultValue(null)]
        public HistorySortOption? SortBy { get; set; }
    }
}
