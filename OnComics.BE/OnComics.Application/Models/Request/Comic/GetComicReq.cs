using OnComics.Application.Enums.Comic;
using OnComics.Application.Models.Request.Common;
using System.ComponentModel;

namespace OnComics.Application.Models.Request.Comic
{
    public class GetComicReq : GetReq
    {
        public Guid? CategoryId { get; set; } = null;

        [DefaultValue(null)]
        public ComicSortOption? SortBy { get; set; }

        [DefaultValue(null)]
        public ComicStatus? Status { get; set; }
    }
}