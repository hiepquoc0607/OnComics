using OnComics.Application.Enums.Comic;
using OnComics.Application.Models.Request.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Comic
{
    public class GetComicReq : GetReq
    {
        [DefaultValue(null)]
        public ComicSortOption? SortBy { get; set; }

        [DefaultValue(null)]
        public ComicStatus? Status { get; set; }
    }
}